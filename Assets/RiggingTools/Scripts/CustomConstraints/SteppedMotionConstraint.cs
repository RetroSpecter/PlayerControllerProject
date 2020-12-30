using System;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Animations;
using Unity.Burst;

[BurstCompile]
public struct SteppedMotionJob : IWeightedAnimationJob
{
    public FloatProperty jobWeight { get; set; }

    public ReadWriteTransformHandle driven;
    public ReadWriteTransformHandle target;

    public AffineTransform offset;
    public float rate;
    public FloatProperty time;
    public float angleThreshold;
    public float distanceThreshold;

    public float rateAhead;

    public void ProcessRootMotion(AnimationStream stream) { }

    public void ProcessAnimation(AnimationStream stream)
    {
        float w = jobWeight.Get(stream);
        if (w > 0f) {
            float angle = Quaternion.Angle(driven.GetLocalRotation(stream), target.GetLocalRotation(stream));
            float dist = Vector3.Distance(driven.GetLocalPosition(stream), target.GetLocalPosition(stream));

            if ((angle > angleThreshold || dist > distanceThreshold && rateAhead-rate/2 < time.Get(stream)) || rateAhead < time.Get(stream)) {
                driven.SetLocalPosition(stream, target.GetLocalPosition(stream));
                driven.SetRotation(stream, target.GetRotation(stream));

                if (rateAhead < time.Get(stream)) {
                    rateAhead += rate;
                }
            }


        }
        else
            AnimationRuntimeUtils.PassThrough(stream, driven);
    }
}

[Serializable]
public struct SteppedMotionData : IAnimationJobData
{

    [SyncSceneToStream] public Transform source;
    [SyncSceneToStream] public Transform target;
    [HideInInspector][SyncSceneToStream] public float time;

    public float angleThreshold;
    public float distanceThreshold;

    public float fps;

    [SyncSceneToStream] public bool maintainOffset;

    public bool IsValid()
    {
        return source != null && target != null;
    }

    public void SetDefaultValues()
    {
        source = null;
        target = null;
        maintainOffset = false;
        fps = 60;
        angleThreshold = 0;
        distanceThreshold = 0;
    }
}

public class SteppedMotionBinder : AnimationJobBinder<SteppedMotionJob, SteppedMotionData>
{
    public override SteppedMotionJob Create(Animator animator, ref SteppedMotionData data, Component component)
    {
        var job = new SteppedMotionJob();

        job.driven = ReadWriteTransformHandle.Bind(animator, data.source);
        job.target = ReadWriteTransformHandle.Bind(animator, data.target);

        job.offset = AffineTransform.identity;

        job.angleThreshold = data.angleThreshold;
        job.distanceThreshold = data.distanceThreshold;
        job.time = FloatProperty.Bind(animator, component, PropertyUtils.ConstructConstraintDataPropertyName(nameof(data.time)));
        job.rate = 1 / data.fps;
        if (data.maintainOffset) {
            job.offset.translation = data.source.position - data.target.position;
        }

        return job;
    }

    public override void Update(SteppedMotionJob job, ref SteppedMotionData data)
    {
        base.Update(job, ref data);
        data.time = Time.time;
    }

    public override void Destroy(SteppedMotionJob job)
    {

    }
}

/// JiggleChain constraint component can be defined given it's job, data and binder
[DisallowMultipleComponent]
public class SteppedMotionConstraint : RigConstraint<SteppedMotionJob, SteppedMotionData, SteppedMotionBinder>
{ }
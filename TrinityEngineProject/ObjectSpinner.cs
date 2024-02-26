using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace TrinityEngineProject
{
    internal class ObjectSpinner : ComponentUpdated
    {
        protected override void Update(FrameEventArgs e)
        {
            base.Update(e);
            transform.rotation = Quaternion.FromEulerAngles(transform.rotation.ToEulerAngles() + Vector3.One * Time.deltaTime);
            //transform.scale = Vector3.One * MathF.Sin(0.0001f * Time.time)*.5f;
            //transform.position = Vector3.One * MathF.Sin(0.0001f * Time.time) * .5f;
        }
    }
}

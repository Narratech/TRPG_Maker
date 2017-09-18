using UnityEngine;
using System.Collections;
using IsoUnity;

namespace IsoUnity.Entities
{
    public class AnimationManager : EventManager
    {

        public override void ReceiveEvent(IGameEvent ev)
        {
            if (ev.Name == "ShowAnimation")
            {
                Decoration dec = (ev.getParameter("Objective") as GameObject).GetComponent<Decoration>();
                GameObject animation = (GameObject)ev.getParameter("Animation");

                GameObject go = (GameObject)GameObject.Instantiate(animation);

                Decoration animation2 = go.GetComponent<Decoration>();

                animation2.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Transparent/Cutout/Diffuse"));
                animation2.Father = dec;
                animation2.adaptate();

                AutoAnimator anim = go.GetComponent<AutoAnimator>();
                anim.registerEvent(ev);
            }


            if (ev.Name == "show decoration animation")
            {
                Decoration dec = (ev.getParameter("objective") as GameObject).GetComponent<Decoration>();

                GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                Decoration decoration = go.AddComponent<Decoration>();
                decoration.IsoDec = (IsoDecoration)ev.getParameter("animation");

                decoration.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("Transparent/Cutout/Diffuse"));
                decoration.Father = dec;
                decoration.adaptate();

                AutoAnimator anim = go.AddComponent<AutoAnimator>();
                anim.FrameRate = 0.1f;
                anim.AutoDestroy = true;
                anim.Repeat = 1;
                anim.registerEvent(ev);

                go.transform.Translate(new Vector3(0, 0, -0.1f));
            }
        }


        public override void Tick() { }

    }
}
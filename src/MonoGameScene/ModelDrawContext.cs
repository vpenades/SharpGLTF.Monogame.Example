﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpGLTF.Runtime;

namespace MonoGameScene
{
    /// <summary>
    /// Small helper for rendering MonoGame models.
    /// </summary>
    class ModelDrawContext
    {
        #region lifecycle                

        public ModelDrawContext(GraphicsDeviceManager graphics, Matrix cameraMatrix)
        {
            _Device = graphics.GraphicsDevice;

            _Device.DepthStencilState = DepthStencilState.Default;

            _View = Matrix.Invert(cameraMatrix);            
            
            float fieldOfView = MathHelper.PiOver4;            
            float nearClipPlane = 0.01f;            
            float farClipPlane = 1000;

            _Projection = Matrix.CreatePerspectiveFieldOfView(fieldOfView, graphics.GraphicsDevice.Viewport.AspectRatio, nearClipPlane, farClipPlane);

            SetDefaultLights();
        }

        #endregion

        #region data

        private GraphicsDevice _Device;
        private Matrix _Projection;
        private Matrix _View;

        private readonly PBRPunctualLight[] _Lights = new PBRPunctualLight[3];        

        #endregion        

        #region API

        public void SetDefaultLights()
        {
            _Lights[0] = PBRPunctualLight.Directional(new Vector3(1, -1, -1), Vector3.One, 3);
            _Lights[1] = PBRPunctualLight.Directional(new Vector3(-1, 0, -1), new Vector3(0.7f, 0.5f, 0.3f), 2);
        }

        public void SetDemoLights(float t)
        {
            var dir = new Vector3((float)Math.Cos(t), 0, -(float)Math.Sin(t));

            _Lights[0] = PBRPunctualLight.Directional(dir, Vector3.One, 1);

            // _Lights[0] = PBRLight.Directional(new Vector3(1, -1, -1), 0, Vector3.One, 9);
            // _Lights[1] = PBRLight.Directional(dir, 0, new Vector3(0.2f, 0.5f, 1f), 5);
            // _Lights[2] = PBRLight.Directional(new Vector3(0, 1, 0), 0, new Vector3(0.5f, 0.2f, 0f), 3);
        }

        public void DrawModelInstance(MonoGameModelInstance model, Matrix world)
        {
            foreach (var e in model.Template.Effects) UpdateMaterial(e);

            model.Draw(_Projection, _View, world);
        }

        public void UpdateMaterial(Effect effect)
        {
            if (effect is IEffectFog fog)
            {
                fog.FogEnabled = false;
            }

            if (effect is IEffectLights classicLights)
            {
                classicLights.LightingEnabled = true;

                classicLights.AmbientLightColor = Vector3.Zero;

                _Lights[0].ApplyTo(classicLights.DirectionalLight0);
                _Lights[1].ApplyTo(classicLights.DirectionalLight1);
                _Lights[2].ApplyTo(classicLights.DirectionalLight2);
            }

            if (effect is PBRPunctualLight.IEffect pbrLights)
            {
                pbrLights.Exposure = 2.5f;

                for(int i=0; i < _Lights.Length; ++i)
                {
                    pbrLights.SetPunctualLight(i, _Lights[i]);
                }
            }
        }

        #endregion
    }
}

﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Xna.Framework.Graphics
{
    public class MetallicRoughnessEffect : PBREffect
    {
        #region lifecycle

        /// <summary>
        /// Creates a new AlphaTestEffect with default parameter settings.
        /// </summary>
        public MetallicRoughnessEffect(GraphicsDevice device) : base(device, Resources.GetShaderByteCode("MetallicRoughnessEffect"))
        {

        }

        #endregion

        #region data       

        private Vector4 _BaseColorScale = Vector4.One;
        private Texture2D _BaseColorMap;

        private Vector2 _MetalRoughnessScale = Vector2.One;
        private Texture2D _MetalRoughnessMap;

        #endregion

        #region properties - material

        public Vector4 BaseColorScale { get => _BaseColorScale; set => _BaseColorScale = value; }

        public Texture2D BaseColorMap { get => _BaseColorMap; set => _BaseColorMap = value; }

        public Vector2 MetalRoughnessScale { get => _MetalRoughnessScale; set => _MetalRoughnessScale = value; }

        public Texture2D MetalRoughnessMap { get => _MetalRoughnessMap; set => _MetalRoughnessMap = value; }

        #endregion

        #region API

        protected override void OnApply()
        {
            base.OnApply();

            ApplyPBR();

            Parameters["PrimaryScale"].SetValue(_BaseColorScale);
            UseTexture("PrimaryTexture", _BaseColorMap ?? Resources.WhiteDotTexture);
            GraphicsDevice.SamplerStates[1] = SamplerState.LinearWrap;

            Parameters["SecondaryScale"].SetValue(_MetalRoughnessScale);
            UseTexture("SecondaryTexture", _MetalRoughnessMap ?? Resources.WhiteDotTexture);
            GraphicsDevice.SamplerStates[2] = SamplerState.LinearWrap;

            var shaderIndex = RecalculateAll();

            CurrentTechnique = Techniques[shaderIndex];
        }

        private int RecalculateAll()
        {
            int techniqueIndex = 0;
            if (BoneCount != 0) techniqueIndex += 1;

            if (NormalMap != null) techniqueIndex += 4;
            if (_BaseColorMap != null) techniqueIndex += 8;
            if (_MetalRoughnessMap != null) techniqueIndex += 16;
            if (EmissiveMap != null) techniqueIndex += 32;
            if (OcclusionMap != null && OcclusionMap != _MetalRoughnessMap) techniqueIndex += 64;

            return techniqueIndex;
        }

        #endregion
    }
}
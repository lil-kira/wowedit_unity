﻿using Assets.Data.WoW_Format_Parsers.WMO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Data.WoW_Format_Parsers.M2
{
    public static class M2_Data
    {
        public static Queue<M2Data> AllM2Data = new Queue<M2Data>();
        public static string[] KeyBoneLookupList = {"ArmL","ArmR", "ShoulderL", "ShoulderR", "SpineLow", "Waist", "Head", "Jaw", "IndexFingerR",
            "MiddleFingerR", "PinkyFingerR", "RingFingerR", "ThumbR", "IndexFingerL", "MiddleFingerL", "PinkyFingerL", "RingFingerL",
            "ThumbL", "$BTH", "$CSR", "$CSL", "_Breath", "_Name", "_NameMount", "$CHD", "$CCH", "Root", "Wheel1", "Wheel2", "Wheel3",
            "Wheel4", "Wheel5", "Wheel6", "Wheel7", "Wheel8"};

        public class M2Data
        {
            // Object //
            public uint FileDataId;
            public string dataPath;
            public int uniqueID;
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 scale;
            public string name;
            public string TextureFilename;

            // Mesh //
            public MeshData meshData = new MeshData();
            public List<SubmeshData> submeshData = new List<SubmeshData>();
            public BoundingBox bounding_box;

            // Material //
            public List<M2BatchIndices> m2BatchIndices = new List<M2BatchIndices>();

            // Texture //
            public List<M2Texture> m2Tex = new List<M2Texture>();
            public List<int> textureLookupTable = new List<int>();

            // Bones //
            public List<M2CompBone> m2CompBone = new List<M2CompBone>();
            public List<int> bone_lookup_table = new List<int>();
            public List<int> key_bone_lookup = new List<int>();

            // Animations //
            public int numberOfAnimations;
            public List<List<Animation_Vector3>> position_animations = new List<List<Animation_Vector3>>();
            public List<List<Animation_Quaternion>> rotation_animations = new List<List<Animation_Quaternion>>();
            public List<List<Animation_Vector3>> scale_animations = new List<List<Animation_Vector3>>();

        }

        public class MeshData
        {
            public List<Vector3> pos = new List<Vector3>();
            public List<float[]> bone_weights = new List<float[]>();
            public List<int[]> bone_indices = new List<int[]>();
            public List<Vector3> normal = new List<Vector3>();
            public List<Vector2> tex_coords = new List<Vector2>();
            public List<Vector2> tex_coords2 = new List<Vector2>();
        }

        public class SubmeshData
        {
            public int ID;
            public Vector3[] vertList;
            public Vector3[] normsList;
            public Vector2[] uvsList;
            public Vector2[] uvs2List;
            public int[] triList;
            public int submesh_StartVertex;

            public BoneWeights[] boneWeights;
            public int submesh_boneCount;
            public int submesh_boneComboIndex;
            public int submesh_boneInfluences;
            public int submesh_centerBoneIndex;
            public Vector3 submesh_centerPosition;
            public Vector3 submesh_sortCenterPosition;
            float submesh_sortRadius;
        }

        public class BoneWeights
        {
            public int[] boneIndex;// = new int[4] {0,0,0,0};
            public float[] boneWeight;// = new float[4] {0f,0f,0f,0f};
        }

        public class Animation_Vector3
        {
            public bool animationExists;
            public List<int> timeStamps;
            public List<Vector3> values;
        }

        public class Animation_Quaternion
        {
            public List<int> timeStamps;
            public List<Quaternion> values;
        }

        public class Keyframe
        {
            public int timeStamp;
            public float value;
        }
    }

    public struct M2Array
    {
        public int Size;
        public int Offset;
    }

    public struct M2Bounds
    {
        public BoundingBox extent;
        public float radius;
    }

    public struct M2TrackBase
    {
        public InterpolationType interpolationtype;
        public int GlobalSequenceID;
        public M2Array Timestamps;
        public M2Array Values;
    }

    public class M2Material
    {
        public List<UInt16> flags = new List<UInt16>();
        public List<UInt16> blending_mode = new List<UInt16>();
    }

    public class M2Texture
    {
        public int type;
        public int flags;
        public M2Array FilenameArray;
        public char[] Filename;
        public uint FileDataId;
        public Texture2Ddata texture2Ddata;
    }

    public class M2BatchIndices
    {
        public int M2Batch_flags;
        public int M2Batch_priorityPlane;
        public int M2Batch_shader_id;
        public int M2Batch_skinSectionIndex;
        public int M2Batch_geosetIndex;
        public int M2Batch_color_index;
        public int M2Batch_materialIndex;
        public int M2Batch_materialLayer;
        public int M2Batch_textureCount;
        public int M2Batch_textureComboIndex;
        public int M2Batch_textureCoordComboIndex;
        public int M2Batch_textureWeightComboIndex;
        public int M2Batch_textureTransformComboIndex;
    }

    public class M2CompBone
    {
        public int key_bone_id;
        public int flags;
        public short parent_bone;
        public ushort submesh_id;
        public ushort uDistToFurthDesc;
        public ushort uZRatioOfChain;
        public Vector3 pivot;
    }


    public enum M2RenderFlags
    {
        unlit = 0x001,
        unfogged = 0x002,
        twoSided = 0x004,
        depthTest = 0x008,
        depthWrite = 0x010,
        shadowBatch = 0x040,
        shadowBatch_2 = 0x080,
        unk = 0x400,
        alpha = 0x800
    }

    public enum M2BlendingMode
    {
        Disabled = 0,
        Mod = 1,
        Decal = 2,
        Add = 3,
        Mod2x = 4,
        Fade = 5,
        DeepRunTram = 6,
        WoDPlus = 7
    }

    public enum InterpolationType : ushort
    {
        None = 0,
        Linear = 1,
        Hermite = 2,
        Bezier = 3
    }

    public enum KeyframeProperty : ushort
    {
        PositionX = 0,
        PositionY = 1,
        PositionZ = 2,
        RotationX = 3,
        RotationY = 4,
        RotationZ = 5,
        RotationW = 6,
        ScaleX = 7,
        ScaleY = 8,
        ScaleZ = 9
    }
}
﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

using Assets.Data.DataLocal;
using Assets.UI.CASC;
using CASCLib;

namespace Assets.Data.WoW_Format_Parsers.WMO
{
    public static partial class WMO
    {
        public static bool ThreadWorking;
        public static GroupData groupDataBuffer;
        public static List<string> LoadedBLPs = new List<string>();

        public static void Load(uint FileDataId, int uniqueID, Vector3 position, Quaternion rotation, Vector3 scale, CASCHandler Handler)
        {
            wmoData = new WMOStruct();

            wmoData.fileDataId = FileDataId;
            wmoData.uniqueID = uniqueID;
            wmoData.position = position;
            wmoData.rotation = rotation;
            wmoData.scale = scale;

            wmoData.Info = new HeaderData();
            wmoData.texturePaths = new Dictionary<uint, uint>();
            wmoData.textureData = new Dictionary<uint, Texture2Ddata>();
            wmoData.MOGNgroupnames = new Dictionary<int, string>();
            wmoData.materials = new List<WMOMaterial>();

            wmoData.groupsData = new List<GroupData>();
            try
            {
                ThreadWorking = true;

                ParseWMO_Root(FileDataId, Handler);

                AllWMOData.Enqueue(wmoData);

                ThreadWorking = false;
            }
            catch (Exception ex)
            {
                Debug.Log("Error : Trying to parse WMO - " + FileDataId);
                Debug.LogException(ex);
            }
        }

        private static void ParseWMO_Root(uint FileDataId, CASCHandler CascHandler)
        {
            long StreamPos = 0;
            using (var stream = CascHandler.OpenFile(FileDataId))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                while (StreamPos < stream.Length)
                {
                    stream.Position = StreamPos;
                    WMOChunkId chunkID = (WMOChunkId)reader.ReadInt32();
                    int chunkSize = reader.ReadInt32();

                    StreamPos = stream.Position + chunkSize;

                    switch (chunkID)
                    {
                        case WMOChunkId.MVER:
                            ReadMVER(reader); // root file version
                            break;
                        case WMOChunkId.MOHD:
                            ReadMOHD(reader); // root file header
                            break;
                        case WMOChunkId.MOMT:
                            ReadMOMT(reader, chunkSize, CascHandler); // materials
                            break;
                        case WMOChunkId.MOUV:
                            ReadMOUV(reader); // texture animation // optional
                            break;
                        case WMOChunkId.MOGN:
                            ReadMOGN(reader, chunkSize); // list of group names
                            break;
                        case WMOChunkId.MOGI:
                            ReadMOGI(reader); // Group information for WMO groups
                            break;
                        case WMOChunkId.MOSB:
                            ReadMOSB(reader, chunkSize); // Skybox model filename
                            break;
                        case WMOChunkId.MOPV:
                            ReadMOPV(reader, chunkSize); // Portal vertices
                            break;
                        case WMOChunkId.MOPT:
                            ReadMOPT(reader); // Portal information
                            break;
                        case WMOChunkId.MOPR:
                            ReadMOPR(reader, chunkSize); // Portal references from groups
                            break;
                        case WMOChunkId.MOVV:
                            ReadMOVV(reader, chunkSize); // Visible block vertices
                            break;
                        case WMOChunkId.MOVB:
                            ReadMOVB(reader, chunkSize); // Visible block list
                            break;
                        case WMOChunkId.MOLT:
                            ReadMOLT(reader); // Lighting information.
                            break;
                        case WMOChunkId.MODS:
                            ReadMODS(reader, chunkSize); // This chunk defines doodad sets.
                            break;
                        case WMOChunkId.MODN:
                            ReadMODN(reader, chunkSize); //List of filenames for M2
                            break;
                        case WMOChunkId.MODD:
                            ReadMODD(reader, chunkSize); // Information for doodad instances.
                            break;
                        case WMOChunkId.MFOG:
                            ReadMFOG(reader, chunkSize); // Fog information.
                            break;
                        case WMOChunkId.MCVP:
                            ReadMCVP(reader, chunkSize); // Convex Volume Planes.
                            break;
                        case WMOChunkId.GFID:
                            ReadGFID(reader, chunkSize); // required when WMO is loaded from fileID
                            break;
                        default:
                            SkipUnknownChunk(stream, chunkID, chunkSize);
                            break;
                    }
                }
                stream.Close();
            }
        }

        private static void ParseWMO_Root(Stream stream)
        {
            
        }

        private static void ParseWMO_Groups(string dataPath)
        {
            wmoData.groupsData = new List<GroupData>(wmoData.Info.nGroups);

            string noExtension = Path.GetFileNameWithoutExtension(dataPath);
            string directoryPath = Path.GetDirectoryName(dataPath);

            //for (int grp = 0; grp < 1; grp++)
            for (int grp = 0; grp < wmoData.Info.nGroups; grp++)
            {
                groupDataBuffer = new GroupData();

                string fileNumber = grp.ToString("000");
                string fullPath = directoryPath + @"\" + noExtension + "_" + fileNumber + ".wmo";

                // using (Stream WMOgroupstream = Casc.GetFileStream(fullPath))
                // using (BinaryReader reader = new BinaryReader(WMOgroupstream))
                // {
                //     int MVER        = reader.ReadInt32();
                //     int MVERsize    = reader.ReadInt32();
                //     ReadMVER(reader); // root file version
                //     int MOGP        = reader.ReadInt32();
                //     int MOGPsize    = reader.ReadInt32();
                //     ReadMOGP(reader); // group header (contains the rest of the chunks)

                //     int MOTVcount = 0;
                //     int MOCVcount = 0;
                //     long streamPosition = WMOgroupstream.Position;
                //     while (streamPosition < WMOgroupstream.Length)
                //     {
                //         WMOgroupstream.Position = streamPosition;
                //         WMOChunkId chunkID  = (WMOChunkId)reader.ReadInt32();
                //         int chunkSize       = reader.ReadInt32();
                //         streamPosition      = WMOgroupstream.Position + chunkSize;
                //         switch (chunkID)
                //         {
                //             case WMOChunkId.MOPY:
                //                 ReadMOPY(reader, chunkSize); // Material info for triangles
                //                 break;
                //             case WMOChunkId.MOVI:
                //                 ReadMOVI(reader, chunkSize); // Vertex indices for triangles
                //                 break;
                //             case WMOChunkId.MOVT:
                //                 ReadMOVT(reader, chunkSize); // Vertices chunk
                //                 break;
                //             case WMOChunkId.MONR:
                //                 ReadMONR(reader, chunkSize); // Normals chunk
                //                 break;
                //             case WMOChunkId.MOTV:
                //                 {
                //                     MOTVcount++;
                //                     if (MOTVcount == 1)
                //                         ReadMOTV(reader, chunkSize); // Texture coordinates
                //                     else if (MOTVcount == 2)
                //                         ReadMOTV2(reader, chunkSize);
                //                     else if (MOTVcount == 3)
                //                         ReadMOTV3(reader, chunkSize);
                //                 }
                //                 break;
                //             case WMOChunkId.MOBA:
                //                 ReadMOBA(reader, chunkSize); // Render batches
                //                 break;
                //             case WMOChunkId.MOCV:
                //                 {
                //                     MOCVcount++;
                //                     if (MOCVcount == 1)
                //                         ReadMOCV(reader, chunkSize);
                //                     else if (MOCVcount == 2)
                //                         ReadMOCV2(reader, chunkSize);
                //                 }
                //                 break;
                //             default:
                //                 SkipUnknownChunk(reader, chunkID, chunkSize);
                //                 break;
                //         }
                //     }
                //     wmoData.groupsData.Add(groupDataBuffer);
                //     WMOgroupstream.Close();
                // }
            }
        }

        public static void SkipUnknownChunk(Stream stream, WMOChunkId chunkID, int chunkSize)
        {
            // Debug.Log("Missing chunk ID : " + chunkID);
            stream.Seek(chunkSize, SeekOrigin.Current);
        }
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utility
{
    public class AvatarUtilities
    {
        public struct SkinnedMeshData
        {
            public Mesh mesh;
            public Material[] materials;
        }

        public static SkinnedMeshRenderer Combine(Transform baseBone,Transform[] bones, SkinnedMeshData[] skinnedMeshes)
        {
            if (skinnedMeshes.Length == 0)
                return null;

            //Collect info from first skinned mesh
            Matrix4x4[] bindPoses = skinnedMeshes[0].mesh.bindposes;
            Dictionary<Material, List<CombineInstance>> combineInstanceArrays = new Dictionary<Material, List<CombineInstance>>();
            Dictionary<Mesh, BoneWeight[]> bone_weights = new Dictionary<Mesh, BoneWeight[]>();

            foreach (SkinnedMeshData skinnedMesh in skinnedMeshes)
            {
                bone_weights[skinnedMesh.mesh] = skinnedMesh.mesh.boneWeights;

                // Prepare stuff for mesh combination with same materials
                for (int i = 0; i < skinnedMesh.mesh.subMeshCount; i++)
                {
                    // Material not in dict, add it
                    if (!combineInstanceArrays.ContainsKey(skinnedMesh.materials[i]))
                        combineInstanceArrays[skinnedMesh.materials[i]] = new List<CombineInstance>();

                    // Add new instance
                    var combine_instance = new CombineInstance();
                    combine_instance.transform = baseBone.localToWorldMatrix;
                    combine_instance.subMeshIndex = i;
                    combine_instance.mesh = skinnedMesh.mesh;

                    combineInstanceArrays[skinnedMesh.materials[i]].Add(combine_instance);
                }
            }

            // Combine meshes into one
            var combined_new_mesh = new Mesh();
            var combined_vertices = new List<Vector3>();
            var combined_uvs = new List<Vector2>();
            var combined_indices = new List<int[]>();
            var combined_bone_weights = new List<BoneWeight>();
            var combined_materials = new Material[combineInstanceArrays.Count];

            var vertex_offset_map = new Dictionary<Mesh, int>();

            int vertex_index_offset = 0;
            int current_material_index = 0;

            foreach (var combine_instance in combineInstanceArrays)
            {
                combined_materials[current_material_index++] = combine_instance.Key;
                var submesh_indices = new List<int>();
                // Process meshes for each material
                foreach (var combine in combine_instance.Value)
                {
                    // Update vertex offset for current mesh
                    if (!vertex_offset_map.ContainsKey(combine.mesh))
                    {
                        // Add vertices for mesh
                        combined_vertices.AddRange(combine.mesh.vertices);
                        // Set uvs
                        combined_uvs.AddRange(combine.mesh.uv);
                        // Add weights
                        combined_bone_weights.AddRange(bone_weights[combine.mesh]);

                        vertex_offset_map[combine.mesh] = vertex_index_offset;
                        vertex_index_offset += combine.mesh.vertexCount;
                    }
                    int vertex_current_offset = vertex_offset_map[combine.mesh];

                    var indices = combine.mesh.GetTriangles(combine.subMeshIndex);
                    // Need to "shift" indices
                    for (int k = 0; k < indices.Length; ++k)
                        indices[k] += vertex_current_offset;

                    submesh_indices.AddRange(indices);
                }
                // Push indices for given submesh
                combined_indices.Add(submesh_indices.ToArray());
            }

            combined_new_mesh.vertices = combined_vertices.ToArray();
            combined_new_mesh.uv = combined_uvs.ToArray();
            combined_new_mesh.boneWeights = combined_bone_weights.ToArray();

            combined_new_mesh.subMeshCount = combined_materials.Length;
            for (int i = 0; i < combined_indices.Count; ++i)
                combined_new_mesh.SetTriangles(combined_indices[i], i);

            // Create mesh renderer
            GameObject combined_mesh_go = new GameObject();
            combined_mesh_go.transform.parent = baseBone.parent;
            combined_mesh_go.transform.localPosition = Vector3.zero;

            //Rotate it to (-90,0,0) since that's how models import
            combined_mesh_go.transform.localRotation = Quaternion.Euler(0,0,0);

            SkinnedMeshRenderer combined_skin_mesh_renderer = combined_mesh_go.AddComponent<SkinnedMeshRenderer>();
            combined_skin_mesh_renderer.sharedMesh = combined_new_mesh;
            combined_skin_mesh_renderer.bones = bones;
            combined_skin_mesh_renderer.rootBone = baseBone;
            combined_skin_mesh_renderer.sharedMesh.bindposes = bindPoses;

            combined_skin_mesh_renderer.sharedMesh.RecalculateNormals();
            combined_skin_mesh_renderer.sharedMesh.RecalculateBounds();
            combined_skin_mesh_renderer.sharedMaterials = combined_materials;

            return combined_skin_mesh_renderer;

        }
    }
}

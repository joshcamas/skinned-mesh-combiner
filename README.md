# Replaced with a [faster](https://github.com/joshcamas/fast-skinned-mesh-combiner) version. Seriously, it's A LOT better.

## skinned-mesh-combiner

Most skinned mesh combiners require actual SkinnedMeshComponents to base the data off of - this means it's a bit slower, since if you want to create a character with a lot of skinned mesh parts, you'll need to first create each individual skinned mesh renderer, then you'll need to merge them and destroy the old renderers. 

This solution skips this issue - simply create a single rig, then give it a bunch of meshe datas - it then spits out a single SkinnedMeshComponent.

There are some downsides - by far the biggest one is the fact that each mesh being merged must have the same exact number of boneweights. So this solution is definitely not for everyone.

### Usage

```c#
//Create Rig
GameObject rig = GameObject.Instantiate(myRig);

//Find the "packed" renderer, which has bone info
SkinnedMeshRenderer rigRenderer = rig.GetComponentInChildren<SkinnedMeshRenderer>();
Transform[] bones = rigRenderer.bones
Transform rootBone = rigRenderer.rootBone;

//Then build your array of SkinnedMeshDatas. These are the meshes / materials that you want to merge:

List<SkinnedMeshData> meshes = new List<SkinnedMeshData>();
meshes.Add(new SkinnedMeshData(mesh1,material));
meshes.Add(new SkinnedMeshData(mesh2,material));
meshes.Add(new SkinnedMeshData(mesh3,material));

//Pack! (In this case, by overwriting the already existing renderer)
Combine(rigRenderer,rootBone,bones,meshes)

```


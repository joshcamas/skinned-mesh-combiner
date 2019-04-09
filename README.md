# skinnedmeshcombiner
A Skinned Mesh Combiner that doesn't require actual skinned meshes (it makes sense I swear)

Most skinned mesh combiners require actual SkinnedMeshComponents to base the data off of - this means it's a bit slower, since if you want to create a character with a lot of skinned mesh parts, you'll need to first create each individual skinned mesh renderer, then you'll need to merge them and destroy the old renderers. A waste of time.

This solution skips this issue - it merges meshes and spits out a single SkinnedMeshComponent. This requires an already existance of a rig / bone array.

There are some downsides - by far the biggest one is the fact that each mesh being merged must have the same exact number of boneweights. So this solution is definitely not for everyone.

### Usage

```
//Create Rig
GameObject rig = GameObject.Instantiate(myRig);

//Find the "packed" renderer, which has bone info
SkinnedMeshRenderer rigRenderer = rig.GetComponentInChildren<SkinnedMeshRenderer>();
Transform[] bones = rigRenderer.bones
Transform rootBone = rigRenderer.rootBone;

//Then build your array of SkinnedMeshDatas. These are the meshes / materials that you want to merge:

List<SkinnedMeshDatas> meshes = new List<SkinnedMeshDatas>();
meshes.Add(new SkinnedMeshDatas(mesh1,material));
meshes.Add(new SkinnedMeshDatas(mesh2,material));
meshes.Add(new SkinnedMeshDatas(mesh3,material));

//Pack! (In this case, by overwriting the already existing renderer)
Combine(rigRenderer,rootBone,bones,meshes)

```


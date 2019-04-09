# skinnedmeshcombiner
A Skinned Mesh Combiner that doesn't require actual skinned meshes (it makes sense I swear)

Most skinned mesh combiners require actual SkinnedMeshComponents to base the data off of - this means it's a bit slower, since if you want to create a character with a lot of skinned mesh parts, you'll need to first create each individual skinned mesh renderer, then you'll need to merge them and destroy the old renderers. A waste of time.

This solution skips this - it merges meshes and creates a single SkinnedMeshComponent for each material.

There are some downsides - by far the biggest one is the fact that each mesh being merged must have the same exact number of boneweights. So this solution is definitely not for everyone.


local growingTrees = treesData.growingTrees
local numGrowingTrees = #growingTrees
local growingTreeIndex = 1
while growingTreeIndex <= numGrowingTrees do
    local tree = growingTrees[growingTreeIndex]
    local treeTypeIndex = tree.treeTypeIndex
    local animal = self.pooledAnimals:getOrCreateNext()
    animal:setPosition(tree.x, tree.y, tree.z)
    growingTreeIndex = growingTreeIndex + 1
end
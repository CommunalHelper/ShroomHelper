local fakeTilesHelper = require("helpers.fake_tiles")

local crumble_block_on_touch = {}

crumble_block_on_touch.name = "ShroomHelper/CrumbleBlockOnTouch" 
crumble_block_on_touch.depth = 0
crumble_block_on_touch.placements = {
    name = "crumble_block_on_touch",
    data = {
        tiletype = "3",
        width = 8,
        height = 8,
        blendin = true,
        persistent = false,
        delay = 0.1,
    }
}

crumble_block_on_touch.sprite = fakeTilesHelper.getEntitySpriteFunction("tiletype", "blendin")
crumble_block_on_touch.fieldInformation = fakeTilesHelper.getFieldInformation("tiletype")

return crumble_block_on_touch
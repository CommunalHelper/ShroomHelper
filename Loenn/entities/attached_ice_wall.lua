local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local attached_ice_wall = {}

attached_ice_wall.name = "ShroomHelper/AttachedIceWall"
attached_ice_wall.depth = 1999
attached_ice_wall.canResize = {false, true}

attached_ice_wall.fieldInformation = {
    spriteOffset = {
        fieldType = "integer"
    }
}

attached_ice_wall.placements = {
    {
        name = "attached_ice_wall_right",
        placementType = "rectangle",
        data = {
            height = 8,
            spriteOffset = 0,
            left = true
        }
    },
    {
        name = "attached_ice_wall_left",
        placementType = "rectangle",
        data = {
            height = 8,
            spriteOffset = 0,
            left = false
        }
    }
}

-- fun fact! the rest of this file is stolen nearly line for line from the plugin of the non-attached version from the base game
-- this code is really interesting and i like it. thanks cruor or vex idk who made it

local iceTopTexture = "objects/wallBooster/iceTop00"
local iceMiddleTexture = "objects/wallBooster/iceMid00"
local iceBottomTexture = "objects/wallBooster/iceBottom00"

function attached_ice_wall.sprite(room, entity)
    local sprites = {}

    local left = entity.left
    local fixedOffset = left and -entity.spriteOffset or entity.spriteOffset
    local height = entity.height or 8
    local tileHeight = math.floor(height / 8)
    local offsetX = left and 0 + fixedOffset or 8 + fixedOffset
    local scaleX = left and 1 or -1

    for i = 2, tileHeight - 1 do
        local middleSprite = drawableSprite.fromTexture(iceMiddleTexture, entity)

        middleSprite:addPosition(offsetX, (i - 1) * 8)
        middleSprite:setScale(scaleX, 1)
        middleSprite:setJustification(0.0, 0.0)

        table.insert(sprites, middleSprite)
    end

    local topSprite = drawableSprite.fromTexture(iceTopTexture, entity)
    local bottomSprite = drawableSprite.fromTexture(iceBottomTexture, entity)

    topSprite:addPosition(offsetX, 0)
    topSprite:setScale(scaleX, 1)
    topSprite:setJustification(0.0, 0.0)

    bottomSprite:addPosition(offsetX, (tileHeight - 1) * 8)
    bottomSprite:setScale(scaleX, 1)
    bottomSprite:setJustification(0.0, 0.0)

    table.insert(sprites, topSprite)
    table.insert(sprites, bottomSprite)

    return sprites
end

function attached_ice_wall.rectangle(room, entity)
    return utils.rectangle(entity.x, entity.y, 8, entity.height or 8)
end

return attached_ice_wall
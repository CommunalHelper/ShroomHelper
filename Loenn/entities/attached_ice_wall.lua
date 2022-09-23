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

local iceTopTexture = "objects/wallBooster/iceTop00"
local iceMiddleTexture = "objects/wallBooster/iceMid00"
local iceBottomTexture = "objects/wallBooster/iceBottom00"

function attached_ice_wall.sprite(room, entity)
    local sprites = {}

    local left = entity.left
    local spriteOffset = entity.spriteOffset or 0
    local height = entity.height or 8
    local tileHeight = math.floor(height / 8)
    local offsetX = 0
    local scaleX = 1
    
    if not entity.left then
        offsetX = 8
        scaleX = -1
        spriteOffset = -spriteOffset
    end

    if spriteOffset ~= 0 then
        add_ice_wall_sprite(entity, sprites, tileHeight, offsetX - spriteOffset, scaleX, 0.4)
    end
    
    add_ice_wall_sprite(entity, sprites, tileHeight, offsetX, scaleX, 1.0)

    return sprites
end

function add_ice_wall_sprite(entity, spriteTable, tileHeight, offsetX, scaleX, alpha)
    for i = 2, tileHeight - 1 do
        local middleSprite = drawableSprite.fromTexture(iceMiddleTexture, entity)

        middleSprite:addPosition(offsetX, (i - 1) * 8)
        middleSprite:setScale(scaleX, 1)
        middleSprite:setJustification(0.0, 0.0)
        middleSprite:setColor({1.0, 1.0, 1.0, alpha})

        table.insert(spriteTable, middleSprite)
    end

    local topSprite = drawableSprite.fromTexture(iceTopTexture, entity)
    local bottomSprite = drawableSprite.fromTexture(iceBottomTexture, entity)

    topSprite:addPosition(offsetX, 0)
    topSprite:setScale(scaleX, 1)
    topSprite:setJustification(0.0, 0.0)
    topSprite:setColor({1.0, 1.0, 1.0, alpha})

    bottomSprite:addPosition(offsetX, (tileHeight - 1) * 8)
    bottomSprite:setScale(scaleX, 1)
    bottomSprite:setJustification(0.0, 0.0)
    bottomSprite:setColor({1.0, 1.0, 1.0, alpha})

    table.insert(spriteTable, topSprite)
    table.insert(spriteTable, bottomSprite)
end

function attached_ice_wall.rectangle(room, entity)
    return utils.rectangle(entity.x, entity.y, 8, entity.height or 8)
end

return attached_ice_wall
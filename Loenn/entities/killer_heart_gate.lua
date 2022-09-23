local drawableSprite = require("structs.drawable_sprite")
local drawableRectangle = require("structs.drawable_rectangle")
local utils = require("utils")
local drawing = require("utils.drawing")


local killer_heart_gate = {}

killer_heart_gate.name = "ShroomHelper/KillerHeartGate"
killer_heart_gate.depth = 0
killer_heart_gate.minimumSize = {40, 8}
killer_heart_gate.fieldInformation = {
    customColor = {
        fieldType = "color"
    }
}
killer_heart_gate.placements = { 
    name = "killer_heart_gate",
    data = {
        width = 40,
        customColor = "18668f",
        triggerDistance = 100,
    }   
}

local edgeTexture = "objects/heartdoor/edge"

function killer_heart_gate.sprite(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width = entity.width or 40
    local roomWidth, roomHeight = room.width, room.height
    local color = entity.customColor

    local edgeSpriteSample = drawableSprite.fromTexture(edgeTexture, entity)
    local edgeWidth, edgeHeight = edgeSpriteSample.meta.width, edgeSpriteSample.meta.height

    local rectangleSprite = drawableRectangle.fromRectangle("fill", x, 0, width, roomHeight, color)
    local sprites = {rectangleSprite}

    local position = {
        x = x,
        y = 0
    }

    for i = 0, roomHeight - 1, edgeHeight do
        local leftSprite = drawableSprite.fromTexture(edgeTexture, position)
        local rightSprite = drawableSprite.fromTexture(edgeTexture, position)

        leftSprite:setJustification(0.5, 0.0)
        leftSprite:setScale(-1, 1)
        leftSprite:addPosition(edgeWidth - 1, i)

        rightSprite:setJustification(0.5, 0.0)
        rightSprite:addPosition(width - edgeWidth + 1, i)

        table.insert(sprites, leftSprite)
        table.insert(sprites, rightSprite)
    end

    return sprites
end

function killer_heart_gate.selection(room, entity)
    local x, y = entity.x or 0, entity.y or 0
    local width = entity.width or 40
    local roomWidth, roomHeight = room.width, room.height

    local mainRectangle = utils.rectangle(x, 0, width, roomHeight)

    return mainRectangle
end

return killer_heart_gate
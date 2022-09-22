local drawableSprite = require("structs.drawable_sprite")
local enums = require("consts.celeste_enums")
local utils = require("utils")

local shroom_dash_switch = {}

shroom_dash_switch.name = "ShroomHelper/ShroomDashSwitch"
shroom_dash_switch.depth = 0

local dash_switch_sides = {
    "up",
    "down",
    "left",
    "right"
}

shroom_dash_switch.fieldInformation = {
    isWindTrigger = {
        options = enums.wind_patterns,
        editable = false
    },
    side = {
        options = dash_switch_sides,
        editable = false
    }
}

shroom_dash_switch.placements = {
    {
        name = "shroom_dash_switch_up",
        data = {
            side = "up",
            persistent = false,
            refillDashOnCollision = false,
            doubleDashRefill = false,
            isWindTrigger = false,
            windPatternOnCollision = "None"
        }
    },
    {
        name = "shroom_dash_switch_down",
        data = {
            side = "down",
            persistent = false,
            refillDashOnCollision = false,
            doubleDashRefill = false,
            isWindTrigger = false,
            windPatternOnCollision = "None"
        }
    },
    {
        name = "shroom_dash_switch_left",
        data = {
            side = "left",
            persistent = false,
            refillDashOnCollision = false,
            doubleDashRefill = false,
            isWindTrigger = false,
            windPatternOnCollision = "None"
        }
    },
    {
        name = "shroom_dash_switch_right",
        data = {
            side = "right",
            persistent = false,
            refillDashOnCollision = false,
            doubleDashRefill = false,
            isWindTrigger = false,
            windPatternOnCollision = "None"
        }
    },
}

local doubleRefillTexture = "objects/sh_dashswitchdoublerefill/dashButtonMirror00"
local singleRefillTexture = "objects/sh_dashswitchrefill/dashButtonMirror00"
local noRefillTexture = "objects/sh_dashswitch/dashButtonMirror00"

local function getSwitchTexture(entity)
    local refill = entity.refillDashOnCollision
    local double = entity.doubleDashRefill

    if double and refill then
        return doubleRefillTexture
    elseif refill then
        return singleRefillTexture
    else
        return noRefillTexture
    end
end

function shroom_dash_switch.selection(room, entity)
    local side = entity.side

    if side == "right" then
        return utils.rectangle(entity.x - 2, entity.y - 1, 12, 18)
    elseif side == "left" then
        return utils.rectangle(entity.x - 2, entity.y - 1, 12, 18)
    elseif side == "up" then
        return utils.rectangle(entity.x - 1, entity.y - 2, 18, 12)
    elseif side == "down" then
        return utils.rectangle(entity.x - 1, entity.y - 2, 18, 12)
    end
end

function shroom_dash_switch.sprite(room, entity)
    local side = entity.side

    local switchSprite = drawableSprite.fromTexture(getSwitchTexture(entity), entity)

    if side == "up" then
        switchSprite:setJustification(0.5, 1)
        switchSprite:addPosition(-16, 8)
        switchSprite.rotation = math.pi / 2
    elseif side == "right" then
        switchSprite:setJustification(0, 0.5)
        switchSprite:addPosition(24, 8)
        switchSprite.rotation = math.pi
    elseif side == "down" then
        switchSprite:setJustification(0.5, 0)
        switchSprite:addPosition(-16, 0)
        switchSprite.rotation = -math.pi / 2
    elseif side == "left" then
        -- 
        switchSprite:setJustification(1, 0.5)
        switchSprite:addPosition(32, 8)
    end

    return switchSprite
end

return shroom_dash_switch

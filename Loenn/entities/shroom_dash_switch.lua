local drawableSprite = require("structs.drawable_sprite")
local enums = require("consts.celeste_enums")
local utils = require("utils")

local shroom_dash_switch = {}

shroom_dash_switch.name = "ShroomHelper/ShroomDashSwitch"
shroom_dash_switch.depth = 0

shroom_dash_switch.fieldInformation = {
    windPatternOnCollision = {
        options = enums.wind_patterns,
        editable = false
    },
    side = {
        options = enums.dash_switch_sides,
        editable = false
    }
}

shroom_dash_switch.fieldOrder = { 
    "x", "y", "side", "windPatternOnCollision", "refillDashOnCollision", "doubleDashRefill", "isWindTrigger", "persistent"
}

shroom_dash_switch.placements = {
    {
        name = "shroom_dash_switch_up",
        data = {
            side = "Up",
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
            side = "Down",
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
            side = "Left",
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
            side = "Right",
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

    if side == "Right" then
        return utils.rectangle(entity.x - 2, entity.y - 1, 12, 18)
    elseif side == "Left" then
        return utils.rectangle(entity.x - 2, entity.y - 1, 12, 18)
    elseif side == "Up" then
        return utils.rectangle(entity.x - 1, entity.y - 2, 18, 12)
    elseif side == "Down" then
        return utils.rectangle(entity.x - 1, entity.y - 2, 18, 12)
    end
end

function shroom_dash_switch.sprite(room, entity)
    local side = entity.side

    local switchSprite = drawableSprite.fromTexture(getSwitchTexture(entity), entity)

    if side == "Up" then
        switchSprite:setJustification(0.5, 1)
        switchSprite:addPosition(-16, 8)
        switchSprite.rotation = math.pi / 2
    elseif side == "Right" then
        switchSprite:setJustification(0, 0.5)
        switchSprite:addPosition(24, 8)
        switchSprite.rotation = math.pi
    elseif side == "Down" then
        switchSprite:setJustification(0.5, 0)
        switchSprite:addPosition(-16, 0)
        switchSprite.rotation = -math.pi / 2
    elseif side == "Left" then
        -- 
        switchSprite:setJustification(1, 0.5)
        switchSprite:addPosition(32, 8)
    end

    return switchSprite
end

return shroom_dash_switch

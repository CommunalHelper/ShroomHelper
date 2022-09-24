local utils = require("utils")

local double_refill_booster = {}

double_refill_booster.name = "ShroomHelper/DoubleRefillBooster"
double_refill_booster.depth = -8500
double_refill_booster.placements = {
    name = "double_refill_booster"
}

double_refill_booster.texture = "objects/sh_doublerefillbooster/boosterPink00"

function double_refill_booster.selection(room, entity)
    return utils.rectangle(entity.x - 9, entity.y - 9, 18, 18)
end

return double_refill_booster
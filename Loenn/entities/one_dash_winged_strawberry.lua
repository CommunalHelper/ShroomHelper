local utils = require("utils")

local one_dash_winged_strawberry = {}

one_dash_winged_strawberry.name = "ShroomHelper/OneDashWingedStrawberry"
one_dash_winged_strawberry.depth = 0
one_dash_winged_strawberry.placements = {
    name = "one_dash_winged_strawberry",
    data = {
        despawnFromSessionIfDashedTwiceInSameRoom = false,
    }
}

one_dash_winged_strawberry.texture = "collectables/ghostgoldberry/wings01"

function one_dash_winged_strawberry.selection(room, entity)
    return utils.rectangle(entity.x - 7, entity.y - 7, 14, 14)
end

return one_dash_winged_strawberry
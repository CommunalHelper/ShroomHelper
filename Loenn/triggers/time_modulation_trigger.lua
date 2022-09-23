local enums = require("consts.celeste_enums")

local time_modulation_trigger = {}

time_modulation_trigger.name = "ShroomHelper/TimeModulationTrigger"

time_modulation_trigger.fieldInformation = {
    positionMode = {
        options = enums.trigger_position_modes,
        editable = false
    }
}

time_modulation_trigger.fieldOrder = { 
    "x", "y", "width", "height", "timeFrom", "timeTo", "positionMode", "destroyOnLeave", "persistent" 
}

time_modulation_trigger.placements = {
    name = "time_modulation_trigger",
    data = {
        timeFrom = 1.0,
        timeTo = 1.0,
        positionMode = "LeftToRight",
        destroyOnLeave = false,
        persistent = false,
    }
}

return time_modulation_trigger
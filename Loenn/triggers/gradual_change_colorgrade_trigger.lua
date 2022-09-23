local enums = require("consts.celeste_enums")

local gradual_change_colorgrade_trigger = {}

gradual_change_colorgrade_trigger.name = "ShroomHelper/GradualChangeColorGradeTrigger"

gradual_change_colorgrade_trigger.fieldInformation = {
    colorGrade = {
        options = enums.color_grades,
        editable = true
    }
}

gradual_change_colorgrade_trigger.placements = {
    name = "gradual_change_colorgrade_trigger",
    data = {
        speed = 1.0,
        colorGrade = "none"
    }
}

return gradual_change_colorgrade_trigger
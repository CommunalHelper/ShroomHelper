local enums = require("consts.celeste_enums")

local multilayer_music_fade_trigger = {}

multilayer_music_fade_trigger.name = "ShroomHelper/MultilayerMusicFadeTrigger"

multilayer_music_fade_trigger.fieldInformation = {
    P1Direction = {
        options = enums.trigger_position_modes,
        editable = false
    },
    P2Direction = {
        options = enums.trigger_position_modes,
        editable = false
    },
    P3Direction = {
        options = enums.trigger_position_modes,
        editable = false
    }
}

multilayer_music_fade_trigger.placements = {
    name = "multilayer_music_fade_trigger",
    data = {
        trackEvent = "",
        P1 = "",
        P2 = "",
        P3 = "",
        P1From = 1.0,
        P2From = 1.0,
        P3From = 1.0,
        P1To = 1.0,
        P2To = 1.0,
        P3To = 1.0,
        P1Direction = "LeftToRight",
        P2Direction = "LeftToRight",
        P3Direction = "LeftToRight",
        persistent = false,
        destroyOnLeave = false
    }
}

return multilayer_music_fade_trigger
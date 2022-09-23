module ShroomHelperMultilayerMusicFadeTrigger

using ..Ahorn, Maple

@mapdef Trigger "ShroomHelper/MultilayerMusicFadeTrigger" MultilayerMusicFadeTrigger(
    x::Integer, 
    y::Integer, 
    width::Integer=Maple.defaultTriggerWidth, 
    height::Integer=Maple.defaultTriggerHeight,
    trackEvent::String="",
    P1::String="",
    P2::String="",
    P3::String="",
    P1From::Number=1.0, 
    P2From::Number=1.0, 
    P3From::Number=1.0, 
    P1To::Number=1.0, 
    P2To::Number=1.0, 
    P3To::Number=1.0, 
    P1Direction::String="LeftToRight",
    P2Direction::String="LeftToRight",
    P3Direction::String="LeftToRight",
    persistent::Bool=false,
    destroyOnLeave::Bool=false,
)

const placements = Ahorn.PlacementDict(
    "Multilayer Music Fade Trigger (Shroom Helper)" => Ahorn.EntityPlacement(
        MultilayerMusicFadeTrigger,
        "rectangle"
    )
)

Ahorn.editingOptions(entity::MultilayerMusicFadeTrigger) = Dict{String, Any}(
    "P1Direction" => Maple.trigger_position_modes,
    "P2Direction" => Maple.trigger_position_modes,
    "P3Direction" => Maple.trigger_position_modes
)

end

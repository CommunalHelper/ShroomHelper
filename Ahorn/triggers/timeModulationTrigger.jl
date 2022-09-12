module ShroomHelperTimeModulationTrigger

using ..Ahorn, Maple

@mapdef Trigger "ShroomHelper/TimeModulationTrigger" TimeModulationTrigger(
	x::Integer, 
	y::Integer, 
	width::Integer=Maple.defaultTriggerWidth, 
	height::Integer=Maple.defaultTriggerHeight, 
	timeFrom::Number=1.0, 
	timeTo::Number=1.0, 
	positionMode::String="LeftToRight",
	destroyOnLeave::Bool=false,
	persistent::Bool=false
)

const placements = Ahorn.PlacementDict(
    "Time Modulation Trigger (Shroom Helper)" => Ahorn.EntityPlacement(
        TimeModulationTrigger,
        "rectangle"
    )
)

Ahorn.editingOptions(entity::TimeModulationTrigger) = Dict{String, Any}(
    "positionMode" => Maple.trigger_position_modes
)

end

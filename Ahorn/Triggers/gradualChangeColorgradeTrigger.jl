module ShroomHelperGradualChangeColorGradeTrigger

using ..Ahorn, Maple

@mapdef Trigger "ShroomHelper/GradualChangeColorGradeTrigger" GradualChangeColorGradeTrigger(x::Integer, y::Integer, width::Integer=Maple.defaultTriggerWidth, height::Integer=Maple.defaultTriggerHeight,
    speed::Number=1.0, colorGrade::String="none")

const placements = Ahorn.PlacementDict(
    "Gradual Change Color Grade Trigger (Shroom Helper)" => Ahorn.EntityPlacement(
        GradualChangeColorGradeTrigger,
        "rectangle"
    )
)

end

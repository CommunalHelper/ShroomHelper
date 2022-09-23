module ShroomHelperRealityDistortionField

using ..Ahorn, Maple

@mapdef Entity "ShroomHelper/RealityDistortionField" RealityDistortionField(
    x::Integer, 
    y::Integer, 
    width::Integer=Maple.defaultBlockWidth, 
    height::Integer=Maple.defaultBlockHeight,
    rippleAreaMultiplier::Number=1.0
)

const placements = Ahorn.PlacementDict(
    "Reality Distortion Field (Shroom Helper)" => Ahorn.EntityPlacement(
        RealityDistortionField,
        "rectangle",
        Dict{String, Any}(),
    ),
)

Ahorn.minimumSize(entity::RealityDistortionField) = 8, 8
Ahorn.resizable(entity::RealityDistortionField) = true, true

function Ahorn.selection(entity::RealityDistortionField)
    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 8))
    height = Int(get(entity.data, "height", 8))

    return Ahorn.Rectangle(x, y, width, height)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::RealityDistortionField, room::Maple.Room)
    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))
    
    rawColor = Ahorn.argb32ToRGBATuple(parse(Int, "0000ff", base=16))[1:3] ./ 255
    realColor = (rawColor..., 0.8)

    Ahorn.drawRectangle(ctx, 0, 0, width, height, realColor, (0.0, 0.0, 0.0, 0.0))
end

end
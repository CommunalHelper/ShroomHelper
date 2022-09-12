module ShroomHelperShroomBookInteraction

using ..Ahorn, Maple

@mapdef Entity "ShroomHelper/ShroomBookInteraction" ShroomBookInteraction(
	x::Integer,
	y::Integer,
	width::Integer=Maple.defaultBlockWidth,
	height::Integer=Maple.defaultBlockHeight, 
	assetKey::String="shroompage"
)

const placements = Ahorn.PlacementDict(
    "Shroom Book Interaction (Shroom Helper)" => Ahorn.EntityPlacement(
        ShroomBookInteraction,
        "rectangle",
		Dict{String, Any}()
    ),
)

Ahorn.minimumSize(entity::ShroomBookInteraction) = 8, 8
Ahorn.resizable(entity::ShroomBookInteraction) = true, true

function Ahorn.selection(entity::ShroomBookInteraction)
    x, y = Ahorn.position(entity)

    width = Int(get(entity.data, "width", 8))
    height = Int(get(entity.data, "height", 8))

    return Ahorn.Rectangle(x, y, width, height)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::ShroomBookInteraction, room::Maple.Room)
    width = Int(get(entity.data, "width", 32))
    height = Int(get(entity.data, "height", 32))
    
	rawColor = Ahorn.argb32ToRGBATuple(parse(Int, "6a0dad", base=16))[1:3] ./ 255
    realColor = (rawColor..., 0.8)

    Ahorn.drawRectangle(ctx, 0, 0, width, height, realColor, (0.0, 0.0, 0.0, 0.0))
end

end
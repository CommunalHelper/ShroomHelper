module ShroomHelperCrumbleBlockOnTouch

using ..Ahorn, Maple

@mapdef Entity "ShroomHelper/CrumbleBlockOnTouch" CrumbleBlockOnTouch(
    x::Integer,
    y::Integer,
    width::Integer=Maple.defaultBlockWidth,
    height::Integer=Maple.defaultBlockHeight,
    blendin::Bool=true,
    persistent::Bool=false,
    delay::Number=0.1,
    destroyStaticMovers::Bool=false
)

const placements = Ahorn.PlacementDict(
    "Crumble Block On Touch (Shroom Helper)" => Ahorn.EntityPlacement(
        CrumbleBlockOnTouch,
        "rectangle",
        Dict{String, Any}(),
        Ahorn.tileEntityFinalizer
    )
)

Ahorn.editingOptions(entity::CrumbleBlockOnTouch) = Dict{String, Any}(
    "tiletype" => Ahorn.tiletypeEditingOptions()
)

Ahorn.minimumSize(entity::CrumbleBlockOnTouch) = 8, 8
Ahorn.resizable(entity::CrumbleBlockOnTouch) = true, true

Ahorn.selection(entity::CrumbleBlockOnTouch) = Ahorn.getEntityRectangle(entity)

Ahorn.renderAbs(ctx::Ahorn.Cairo.CairoContext, entity::CrumbleBlockOnTouch, room::Maple.Room) = Ahorn.drawTileEntity(ctx, room, entity)

end
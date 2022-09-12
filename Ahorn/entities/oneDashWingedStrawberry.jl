module ShroomHelperOneDashWingedStrawberry

using ..Ahorn, Maple

@mapdef Entity "ShroomHelper/OneDashWingedStrawberry" OneDashWingedStrawberry(x::Integer, y::Integer, despawnFromSessionIfDashedTwiceInSameRoom::Bool=false)

const placements = Ahorn.PlacementDict(
    "One Dash Winged Strawberry (Shroom Helper)" => Ahorn.EntityPlacement(
        OneDashWingedStrawberry
    )
)

function Ahorn.selection(entity::OneDashWingedStrawberry)
    x, y = Ahorn.position(entity)
    return Ahorn.Rectangle(x - 7, y - 7, 14, 14)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::OneDashWingedStrawberry, room::Maple.Room)
    Ahorn.drawSprite(ctx, "collectables/ghostgoldberry/wings01", 0, 0)
end

end
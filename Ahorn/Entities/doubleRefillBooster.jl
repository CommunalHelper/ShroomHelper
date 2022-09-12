module ShroomHelperDoubleRefillBooster

using ..Ahorn, Maple

@mapdef Entity "ShroomHelper/DoubleRefillBooster" DoubleRefillBooster(x::Integer, y::Integer)

const placements = Ahorn.PlacementDict(
    "Double Refill Booster (Shroom Helper)" => Ahorn.EntityPlacement(
        DoubleRefillBooster
    )
)

function boosterSprite(entity::DoubleRefillBooster)
	return "objects/sh_doublerefillbooster/boosterPink00"
end

function Ahorn.selection(entity::DoubleRefillBooster)
    x, y = Ahorn.position(entity)
    sprite = boosterSprite(entity)
    return Ahorn.getSpriteRectangle(sprite, x, y)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::DoubleRefillBooster, room::Maple.Room)
    sprite = boosterSprite(entity)
    Ahorn.drawSprite(ctx, sprite, 0, 0)
end

end
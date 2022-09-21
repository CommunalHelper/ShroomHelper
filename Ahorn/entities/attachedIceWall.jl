module ShroomHelperAttachedIceWall

using ..Ahorn, Maple

@mapdef Entity "ShroomHelper/AttachedIceWall" AttachedIceWall(
	x::Integer,
	y::Integer,
	width::Integer=Maple.defaultBlockWidth,
	height::Integer=Maple.defaultBlockHeight,
	spriteOffset::Integer=0,
	left::Bool=true
)

const placements = Ahorn.PlacementDict(
    "Attached Ice Wall (Right) (Shroom Helper)" => Ahorn.EntityPlacement(
        AttachedIceWall,
        "rectangle",
        Dict{String, Any}(
            "left" => true
        )
    ),
    "Attached Ice Wall (Left) (Shroom Helper)" => Ahorn.EntityPlacement(
        AttachedIceWall,
        "rectangle",
        Dict{String, Any}(
            "left" => false
        )
    )
)

Ahorn.minimumSize(entity::AttachedIceWall) = 0, 8
Ahorn.resizable(entity::AttachedIceWall) = false, true

function Ahorn.selection(entity::AttachedIceWall)
    x, y = Ahorn.position(entity)
    height = Int(get(entity.data, "height", 8))

    return Ahorn.Rectangle(x, y, 8, height)
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::AttachedIceWall, room::Maple.Room)
    left = get(entity.data, "left", false)
    offset = get(entity.data, "spriteOffset", 0)

    # Values need to be system specific integer
    x = Int(get(entity.data, "x", 0))
    y = Int(get(entity.data, "y", 0))

    height = Int(get(entity.data, "height", 8))
    tileHeight = div(height, 8)

    if left
        for i in 2:tileHeight - 1
            Ahorn.drawImage(ctx, "objects/wallBooster/iceMid00", 0 + -offset, (i - 1) * 8)
        end

        Ahorn.drawImage(ctx, "objects/wallBooster/iceTop00", 0 + -offset, 0)
        Ahorn.drawImage(ctx, "objects/wallBooster/iceBottom00", 0 + -offset, (tileHeight - 1) * 8)

    else
        Ahorn.Cairo.save(ctx)
        Ahorn.scale(ctx, -1, 1)

        for i in 2:tileHeight - 1
            Ahorn.drawImage(ctx, "objects/wallBooster/iceMid00", -8 + -offset, (i - 1) * 8)
        end

        Ahorn.drawImage(ctx, "objects/wallBooster/iceTop00", -8 + -offset, 0)
        Ahorn.drawImage(ctx, "objects/wallBooster/iceBottom00", -8 + -offset, (tileHeight - 1) * 8)

        Ahorn.restore(ctx)
    end
end

end
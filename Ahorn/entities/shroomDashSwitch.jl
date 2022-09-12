module ShroomHelperShroomDashSwitch

using ..Ahorn, Maple

sides = String[
    "left",
    "right",
    "up",
    "down"
]

textures = String[
    "default",
    "mirror"
]

@mapdef Entity "ShroomHelper/ShroomDashSwitch" ShroomDashSwitch(
    x::Integer, 
    y::Integer, 
    side::String="up",
    persistent::Bool=false,
    refillDashOnCollision::Bool=false,
    doubleDashRefill::Bool=false,
    isWindTrigger::Bool=false,
    windPatternOnCollision::String="None"
)

const placements = Ahorn.PlacementDict(
    "Shroom Dash Switch ($(uppercasefirst(side))) (Shroom Helper)" => Ahorn.EntityPlacement(
        ShroomDashSwitch,
        "rectangle",
        Dict{String, Any}(
            "side" => side
        )
    ) for side in sides
)

Ahorn.editingOptions(entity::ShroomDashSwitch) = Dict{String, Any}(
    "side" => sides,
    "windPatternOnCollision" => Maple.wind_patterns
)

function Ahorn.selection(entity::ShroomDashSwitch)
    x, y = Ahorn.position(entity)
    side = get(entity.data, "side", false)

    if side == "right"
        return Ahorn.Rectangle(x, y - 1, 10, 16)
    elseif side == "left"
        return Ahorn.Rectangle(x - 2, y - 1, 10, 16)
    elseif side == "up"
        return Ahorn.Rectangle(x, y - 4, 16, 12)
    elseif side == "down"
        return Ahorn.Rectangle(x, y, 16, 12)
    end
end

function Ahorn.render(ctx::Ahorn.Cairo.CairoContext, entity::ShroomDashSwitch, room::Maple.Room)
    sprite = get(entity.data, "sprite", "default")
    side = get(entity.data, "side", "up")

    texture = "objects/sh_dashswitch/dashButtonMirror00"

    if (entity.refillDashOnCollision && entity.doubleDashRefill)
        texture = "objects/sh_dashswitchdoublerefill/dashButtonMirror00"
    elseif entity.refillDashOnCollision
        texture = "objects/sh_dashswitchrefill/dashButtonMirror00"
    end

    if side == "left"
        Ahorn.drawSprite(ctx, texture, 8, 7)
    elseif side == "right"
        Ahorn.drawSprite(ctx, texture, 48, 55, rot=pi)
    elseif side == "down"
        Ahorn.drawSprite(ctx, texture, 8, 48, rot=-pi / 2)
    elseif side == "up"
        Ahorn.drawSprite(ctx, texture, 56, 8, rot=pi / 2)
    end
end

end
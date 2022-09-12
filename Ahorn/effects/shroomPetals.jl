module ShroomHelperShroomPetals

using ..Ahorn, Maple

@mapdef Effect "ShroomHelper/ShroomPetals" ShroomPetals(only::String="*", exclude::String="", color::String="161933")

placements = ShroomPetals

function Ahorn.canFgBg(ShroomPetals)
    return true, true
end

end
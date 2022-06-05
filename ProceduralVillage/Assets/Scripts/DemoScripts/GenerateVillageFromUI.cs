using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GenerateVillageFromUI : MonoBehaviour
{
    public DemoCamera gameCamera;

    public CreateObjectFromGraph VillageGenerator;

    public Slider VillageWidthSlider;
    public Slider VillageLengthSlider;

    public Slider VillageBlocksSlider;
    public Slider VillageAreasSlider;
    public Slider VillageSmoothSlider;

    public Slider BuildingMinHeightSlider;
    public Slider BuildingMaxHeightSlider;
    public Slider RoofMinHeightSlider;
    public Slider RoofMaxHeightSlider;
    public Slider MinBuildingArea;

    void Start()
    {
        Generate();
    }


    public void Generate()
    {
        float width = VillageWidthSlider.value;
        float depth = VillageLengthSlider.value;
        int BlockCount = (int) VillageBlocksSlider.value;
        int AreasPerBlock = (int)VillageAreasSlider.value;

        //Point camera to the new center
        gameCamera.SetTarget(width * 0.5f, depth * 0.5f);


        //Update parameter values
        VillageGenerator.SetParameterValue("Village Width", width);
        VillageGenerator.SetParameterValue("Village Length", depth);

        VillageGenerator.SetParameterValue("Block Count", BlockCount);
        VillageGenerator.SetParameterValue("Areas Per Block", AreasPerBlock);
        VillageGenerator.SetParameterValue("Smooth Factor", VillageSmoothSlider.value);

        VillageGenerator.SetParameterValue("Min building height", BuildingMinHeightSlider.value);
        VillageGenerator.SetParameterValue("Max Building Height", BuildingMinHeightSlider.value +  BuildingMaxHeightSlider.value);

        VillageGenerator.SetParameterValue("Min Roof Height", RoofMinHeightSlider.value);
        VillageGenerator.SetParameterValue("Max Roof Height", RoofMinHeightSlider.value + RoofMaxHeightSlider.value);

        float minArea = (2 * width * depth) / (BlockCount * AreasPerBlock) * MinBuildingArea.value;
        VillageGenerator.SetParameterValue("Building Min Area", minArea);

        //Generate
        VillageGenerator.Generate();


    }
}

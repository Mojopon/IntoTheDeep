using UnityEngine;
using System.Collections;
using System;
using UniRx;

public class SkillPreviewer : MonoBehaviour, IMapInstanceUtilitiesUser
{
    public Transform marker;

    public Func<int, int, Vector2> CoordToWorldPositionConverter { get; set; }

    void Start()
    {
        StartCoroutine(GetMapInstanceUtilitiesCoroutine());
    }

    IEnumerator GetMapInstanceUtilitiesCoroutine()
    {
        var mapInstanceObject = GameObject.FindGameObjectWithTag("MapInstance");
        if (mapInstanceObject == null)
        {
            yield return null;
            StartCoroutine(GetMapInstanceUtilitiesCoroutine());
        }
        else
        {
            var mapInstance = mapInstanceObject.GetComponent<IMapInstanceUtilitiesProvider>();
            mapInstance.ProvideMapInstanceUtilities(this);

            StartCoroutine(GetSkillMenuCoroutine());
        }
    }

    IEnumerator GetSkillMenuCoroutine()
    {
        if(SkillMenu.Current == null)
        {
            yield return null;
            StartCoroutine(GetSkillMenuCoroutine());
        }
        else
        {
            SkillMenu.Current.SelectedSkill
                             .Subscribe(x => PreviewSkill(x));
        }
    }

    GameObject previewObjectsHolder;
    void PreviewSkill(SkillInfo skillInfo)
    {
        if (skillInfo == null)
        {
            StopPreviewSkill();
            return;
        }

        var skill = skillInfo.skill;
        if (previewObjectsHolder != null)
        {
            Destroy(previewObjectsHolder);
        }
        previewObjectsHolder = new GameObject("SkillPreviewObjects");

        var user = skillInfo.user;
        foreach(var coord in skill.range)
        {
            var destination = new Coord(coord.x + user.X, coord.y + user.Y);
            var obj = Instantiate(marker, CoordToWorldPositionConverter(destination.x, destination.y), Quaternion.identity) as Transform;
            obj.SetParent(previewObjectsHolder.transform);
        }
    }

    void StopPreviewSkill()
    {
        if(previewObjectsHolder != null)
        {
            Destroy(previewObjectsHolder);
        }
    }
}

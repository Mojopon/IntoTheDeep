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

    private IMapInstanceUtilitiesProvider provider;
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
            provider = mapInstanceObject.GetComponent<IMapInstanceUtilitiesProvider>();
            GetMapInstanceUtilities(provider);

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
                             .Subscribe(x => PreviewSkill(x))
                             .AddTo(gameObject);
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
        foreach(var coord in skill.effects[0].ranges)
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

    public void GetMapInstanceUtilities(IMapInstanceUtilitiesProvider provider)
    {
        CoordToWorldPositionConverter = provider.CoordToWorldPositionConverter;
    }
}

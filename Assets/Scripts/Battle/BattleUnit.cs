
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleUnit : MonoBehaviour
{
    //[SerializeField] PokemonBase _base;
    //[SerializeField] int level;
    [SerializeField] bool isPlayerUnit;
    [SerializeField] Battlehud hud;
    [SerializeField] Image pokemonSprite;
    Vector3 originalPos;
    Color originalColor;

    public bool IsPlayerUnit
    {
        get
        {
            return isPlayerUnit;
        }
    }

    public Battlehud Hud
    {
        get
        {
            return hud;
        }
    }

    public Pokemon Pokemon { get; set; }

    private void Awake()
    {
        originalPos = pokemonSprite.transform.localPosition;
        originalColor = pokemonSprite.color;
    }
    public void Setup(Pokemon pokemon)
    {
        Pokemon = pokemon;
        if (isPlayerUnit)
            pokemonSprite.sprite = Pokemon.Base.BackSprite;
        else
            pokemonSprite.sprite = Pokemon.Base.ForntSprite;

        hud.gameObject.SetActive(true);
        hud.SetData(pokemon);

        pokemonSprite.color = originalColor;
        PlayEnterAnimation();
    }

    public void Clear()
    {
        hud.gameObject.SetActive(false);
    }

    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            pokemonSprite.transform.localPosition = new Vector3(-500, originalPos.y);
        else
            pokemonSprite.transform.localPosition = new Vector3(500, originalPos.y);

        pokemonSprite.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayAttactAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(pokemonSprite.transform.DOLocalMoveX(originalPos.x + 50f, .25f));
        else
            sequence.Append(pokemonSprite.transform.DOLocalMoveX(originalPos.x - 50f, .25f));

        sequence.Append(pokemonSprite.transform.DOLocalMoveX(originalPos.x, .25f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(pokemonSprite.DOColor(Color.gray, .1f));
        sequence.Append(pokemonSprite.DOColor(originalColor, .1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(pokemonSprite.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        sequence.Join(pokemonSprite.DOFade(0f, .5f));
    }

    public IEnumerator PlayCaptureAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(pokemonSprite.DOFade(0, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y + 50f, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(0.3f, 0.3f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }


    public IEnumerator PlayBreakOutAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(pokemonSprite.DOFade(1, 0.5f));
        sequence.Join(transform.DOLocalMoveY(originalPos.y, 0.5f));
        sequence.Join(transform.DOScale(new Vector3(1f, 1f, 1f), 0.5f));
        yield return sequence.WaitForCompletion();
    }

}
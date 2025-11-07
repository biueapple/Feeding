using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMovingTile : MonoBehaviour
{
    public Vector2 scrollSpeed = new Vector2(0.2f, 0f); // x 방향으로 움직임
    public Vector2 tiling = new Vector2(3f, 1f);        // 화면 안에 몇 번 반복할지

    private RawImage rawImage;
    private Rect baseRect;

    private Coroutine coroutine;

    void Awake()
    {
        rawImage = GetComponent<RawImage>();

        if (rawImage.texture == null)
        {
            Debug.LogError("RawImage에 텍스처를 지정하세요.");
            enabled = false;
            return;
        }

        // 시작 uv 설정: (0,0)에서 tiling.x, tiling.y 만큼 반복
        baseRect = new Rect(0, 0, tiling.x, tiling.y);
        rawImage.uvRect = baseRect;
    }

    public void StartMovingTile()
    {
        coroutine = StartCoroutine(MoveingTile());
    }

    public void StopMovingTile()
    {
        if (coroutine == null) return;
        StopCoroutine(coroutine);
        coroutine = null;
    }

    private IEnumerator MoveingTile()
    {
        while(true)
        {
            Rect r = rawImage.uvRect;

            // 시간에 따라 이동
            r.x += scrollSpeed.x * Time.deltaTime;
            r.y += scrollSpeed.y * Time.deltaTime;

            // 반복되도록 감싸기 (안 하면 값이 무한히 커짐)
            r.x = Mathf.Repeat(r.x, 1f);
            r.y = Mathf.Repeat(r.y, 1f);

            // 위치만 바꾸고, 크기는 유지
            r.width = baseRect.width;
            r.height = baseRect.height;

            rawImage.uvRect = r;

            yield return null;
        }
    }
}

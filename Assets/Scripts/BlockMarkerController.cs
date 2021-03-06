﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMarkerController : MonoBehaviour {

  private GameObject firstBlockMarker; //最初にドラッグしたボール
  private GameObject lastBlockMarker; //最後にドラッグしたボール
  GameObject block;
  BlockController blockScript;
  private string currentName; //名前判定用のstring変数
  List<int> chainMarkerX = new List<int>();
  List<int> chainMarkerY = new List<int>();
  GameObject character;

  // Use this for initialization
  void Start () {
    character = GameObject.Find("Character");
    block = GameObject.Find("Stone1");
    blockScript = block.GetComponent<BlockController>();
  }

  // Update is called once per frame
  void Update()
  {
    //画面をクリックし、firstBallがnullの時実行
    if (Input.GetMouseButtonDown(0) && firstBlockMarker == null)
    {
      OnDragStart();
    }
    else if (Input.GetMouseButtonUp(0))
    {
      //クリックを終えた時
      OnDragEnd();
    }
    else if (firstBlockMarker != null)
    {
      OnDragging();
    }
  }

  public void OnDragStart()
  {
    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
    // ヒットしたオブジェクトがマーカーだった場合、キャラとの距離を計算しブロック１つ分の距離だった場合、オブジェウトの1ます下にブロックが存在していればtrue、してなければfalse
    if (hit.collider != null)
    {
	  print("Start");
      GameObject hitObj = hit.collider.gameObject;
      //オブジェクトの名前を前方一致で判定
      string markerName = hitObj.name;
      // キャラの座標取得
      // タップしたブロックがキャラの左下、真下、右下の場合処理を行う
      if (markerName.StartsWith("BlockMarker"))
      {
        // 上を選択した場合はジャンプ
        float distance = Vector2.Distance(hitObj.transform.position, character.transform.position);
        if (distance < 1.0f)
        {
          print("Marker");
          firstBlockMarker = hitObj;
          lastBlockMarker = hitObj;
          currentName = hitObj.name;
          //ドラッグ中のオブジェクトの座標を取得
          int matrixX = blockScript.getMatrix_X(hitObj.transform.position.x);
          int matrixY = blockScript.getMatrix_Y(hitObj.transform.position.y);
          //print("(x,y)=" + "(" + matrixY + ", " + matrixX + ")");
          chainMarkerX.Add(matrixX);
          chainMarkerY.Add(matrixY);
        }
      }
    }
  }

  public void OnDragging()
  {
    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
    // ドラッグしているオブジェクトがマーカーだった場合、オブジェウトの1ます下にブロックが存在していればtrue、してなければfalse
    if (hit.collider != null)
    {
			print("Drag");
      GameObject hitObj = hit.collider.gameObject;
      //オブジェクトの名前を前方一致で判定
      string markerName = hitObj.name;
      if (markerName.StartsWith("BlockMarker"))
      {
        // ブロックマーカーの座標を取得
        int matrixX = blockScript.getMatrix_X(hitObj.transform.position.x);
        int matrixY = blockScript.getMatrix_Y(hitObj.transform.position.y);

        // ドラッグスタートしたブロックマーカーから1マス以内にあるマーカーだった場合処理継続
        float distance = Vector2.Distance(hitObj.transform.position, character.transform.position);
        if (distance < 1.0f)
        {
          // その1ます下にブロックが存在するかを判定
          if (blockScript.isExistBlock(matrixX, matrixY + 1))
          {
            // 存在していた場合は足場として連結リストに加える
            chainMarkerX.Add(matrixX);
            chainMarkerY.Add(matrixY);
            // 線でつなげる
            LineRenderer renderer = gameObject.GetComponent<LineRenderer>();
            // 線の幅
            renderer.SetWidth(0.1f, 0.1f);
            // 頂点の数
            renderer.SetVertexCount(2);
            // 頂点を設定
            renderer.SetPosition(0, new Vector3(blockScript.getPositionX(chainMarkerX[chainMarkerX.Count]), blockScript.getPositionY(chainMarkerY[chainMarkerY.Count]), 0f));// １つ前のマーカー
            renderer.SetPosition(1, new Vector3(hitObj.transform.position.x, hitObj.transform.position.y, 0f));// 現在ドラッグ中のマーカー
          }
        }
      }
    }
  }

    public void OnDragEnd()
    {
		print("END");
		// 移動開始
		// コルーチンで遅延を発生させながら移動させる
		// 全ての線を消す
		StartCoroutine("MoveCharacter");

    }
	IEnumerator MoveCharacter()
	{
		//リストを引数に取り、ループ
        for(int i = 0; i < chainMarkerX.Count; i++)
		{
			iTween.MoveTo(block, iTween.Hash("x", chainMarkerX[i], "y", chainMarkerY[i]));
			//0.5秒停止
			yield return new WaitForSeconds(0.5f);
		}
		
	}
}

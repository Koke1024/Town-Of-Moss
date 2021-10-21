// using System;
// using UnityEngine;
//
// // Token: 0x020000DA RID: 218
// public class PooledMapIcon : PoolableBehavior
// {
// 	// Token: 0x06000537 RID: 1335 RVA: 0x0001FCA0 File Offset: 0x0001DEA0
// 	public void Update()
// 	{
// 		if (this.alphaPulse.enabled)
// 		{
// 			float num = Mathf.Abs(Mathf.Cos((this.alphaPulse.Offset + Time.time) * 3.1415927f / this.alphaPulse.Duration));
// 			if ((double)num > 0.9)
// 			{
// 				num -= 0.9f;
// 				num = this.NormalSize + num;
// 				base.transform.localScale = new Vector3(num, num, num);
// 			}
// 		}
// 	}
//
// 	// Token: 0x06000538 RID: 1336 RVA: 0x0001FD19 File Offset: 0x0001DF19
// 	public void Reset()
// 	{
// 		this.lastMapTaskStep = -1;
// 		this.alphaPulse.enabled = false;
// 		this.rend.material.SetFloat("_Outline", 0f);
// 		base.Reset();
// 	}
//
// 	// Token: 0x040005F0 RID: 1520
// 	public float NormalSize = 0.3f;
//
// 	// Token: 0x040005F1 RID: 1521
// 	public int lastMapTaskStep = -1;
//
// 	// Token: 0x040005F2 RID: 1522
// 	public SpriteRenderer rend;
//
// 	// Token: 0x040005F3 RID: 1523
// 	public AlphaPulse alphaPulse;
// }

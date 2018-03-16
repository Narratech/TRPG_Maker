using UnityEngine;
using System.Collections;

using IsoUnity.Entities;

namespace IsoUnity.Sequences {
	[NodeContent("Fork/Single/Item Fork", 2)]
	public class ItemFork : Checkable {

		public IsoUnity.Entities.Inventory inventory;
		public IsoUnity.Entities.Item item;
		public bool contains;

		public override bool check()
		{
			bool find = false;
			foreach(IsoUnity.Entities.Item item in inventory.Items)
				if(item.isEqualThan(this.item)){
					find = true; break;
				}
			return contains?find:!find;
		}
	}
}
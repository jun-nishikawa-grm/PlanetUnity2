/* Copyright (c) 2012 Small Planet Digital, LLC
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files 
 * (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, 
 * publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE 
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using UnityEngine;
using System;
using System.Reflection;
using System.Collections;

public interface IPUCode {

}

public partial class PUCode : PUCodeBase {

	IPUCode controller;

	private static Hashtable instances = new Hashtable ();

	public object GetObject()
	{
		return controller;
	}

	public void DetachObject()
	{
		controller = null;
	}

	public override void gaxb_unload()
	{
		base.gaxb_unload ();
		NotificationCenter.removeObserver (controller);
	}

	public override void gaxb_init()
	{
		controller = null;
		GC.Collect();
	}

	public static IPUCode GetSingletonByName(string name){
		return (IPUCode)instances [name];
	}

	public void gaxb_complete()
	{
		if (gameObject != null) {
			gameObject.name = _class;
		}

		if (singleton) {
			if (instances [_class] != null && instances [_class] != this) {
				GameObject.Destroy (this.gameObject);
				controller = (IPUCode)instances[_class];
			} else {
				MonoBehaviour.DontDestroyOnLoad(this.gameObject);
				this.gameObject.transform.parent = null;
			}
		}


		if (controller == null && _classExists) {
			// Attach all of the PlanetUnity objects
			try {
				controller = (IPUCode)gameObject.AddComponent(Type.GetType (_class, true));

				PUGameObject scene = Scope() as PUGameObject;
				if(scene != null)
				{
					FieldInfo field = controller.GetType ().GetField ("scene");
					if (field != null)
					{
						field.SetValue (controller, scene);
					}

					field = controller.GetType ().GetField ("puGameObject");
					if (field != null)
					{
						field.SetValue (controller, this);
					}

					scene.PerformOnChildren(val =>
						{
							PUGameObject oo = val as PUGameObject;
							if(oo != null && oo.title != null)
							{
								field = controller.GetType ().GetField (oo.title);
								if (field != null)
								{
									field.SetValue (controller, oo);
								}
							}
							return true;
						});
				}

				if(singleton){
					Debug.Log("Saving instance class for: "+_class);
					instances[_class] = controller;
				}
			}
			catch(Exception e) {
				UnityEngine.Debug.Log ("Controller error: " + e);
			}
		}

		if (controller != null) {
			try {
				// Attach all of the named GameObjects
				FieldInfo[] fields = controller.GetType ().GetFields ();
				foreach (FieldInfo field in fields) {
					if (field.FieldType == typeof(GameObject)) {

						GameObject[] pAllObjects = (GameObject[])Resources.FindObjectsOfTypeAll (typeof(GameObject));

						foreach (GameObject pObject in pAllObjects) {
							if (pObject.name.Equals (field.Name)) {
								field.SetValue (controller, pObject);
							}
						}
					}
				}
			} catch (Exception e) {
				UnityEngine.Debug.Log ("Controller error: " + e);
			}
		
			foreach (PUNotification subscribe in Notifications) {
				NotificationCenter.addObserver (controller, subscribe.name, Scope (), subscribe.name);
			}
		}

		base.gaxb_complete ();
	}

}

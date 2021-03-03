using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Tests
{
    public class TestSuite: InputTestFixture
    {
        GameObject playerPrefab   = Resources.Load<GameObject>("Player");
        GameObject skeletonPrefab = Resources.Load<GameObject>("ArmedSkeleton");
        Mouse mouse;
        Keyboard keyboard;

        public override void Setup()
        {
            base.Setup();
            SceneManager.LoadScene("Scenes/Sandbox");
            playerPrefab.GetComponent<Player>().activeCam = false;

            mouse = InputSystem.AddDevice<Mouse>();
            keyboard = InputSystem.AddDevice<Keyboard>();
        }


        [Test]
        public void TestPlayerDamage()
        {
            Vector3 playerPos = Vector3.zero;
            Quaternion playerDir = Quaternion.identity; // the default direction the player is facing is enough
            GameObject player = GameObject.Instantiate(playerPrefab, playerPos, playerDir);

            Assert.That(player.GetComponent<Player>().health, Is.EqualTo(100f));

            player.GetComponent<Player>().applyDamage(20f);

            Assert.That(player.GetComponent<Player>().health, Is.EqualTo(80f));
        }

        [UnityTest]
        public IEnumerator TestSlashDamagesSkeleton()
        {
            Vector3 playerPos = new Vector3(2f, 1f, -1f);
            Quaternion playerDir = Quaternion.identity;
            Vector3 skeletonPos = new Vector3(2f, 0f, 1f);
            Quaternion skeletonDir = Quaternion.LookRotation(new Vector3(0f, 0f, -1f), Vector3.up);

            GameObject player = GameObject.Instantiate(playerPrefab, playerPos, playerDir);
            GameObject skeleton = GameObject.Instantiate(skeletonPrefab, skeletonPos, skeletonDir);
            skeleton.GetComponent<Skeleton>().player = player.GetComponent<Player>();
            skeleton.GetComponent<Skeleton>().enabled = false;

            Assert.That(skeleton.GetComponent<Skeleton>().health, Is.EqualTo(100f));

            Press(mouse.leftButton);
            yield return new WaitForSeconds(0.1f);
            Release(mouse.leftButton);
            yield return new WaitForSeconds(3f);

            Assert.That(skeleton.GetComponent<Skeleton>().health, Is.EqualTo(80f));
        }
    }
}

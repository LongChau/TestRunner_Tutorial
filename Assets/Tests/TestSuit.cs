using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class TestSuite
{
    private Game game;

    // Tearing down.
    // Use for common code part between testcases.
    // The SetUp attribute specifies that this method is called before each test is run.
    [SetUp]
    public void Setup()
    {
        GameObject gameObject =
            Object.Instantiate(Resources.Load<GameObject>("Prefabs/Game"));
        game = gameObject.GetComponent<Game>();
    }

    // The TearDown attribute specifies that this method is called after each test is run. 
    [TearDown]
    public void Teardown()
    {
        Object.Destroy(game.gameObject);
    }

    // 1: This Attribute tells Unity that this is a unit test. This will appear in TestRunner
    [UnityTest]
    public IEnumerator AsteroidsMoveDown()
    {
        // UPDATED: Because of [Setup] so we delete or don't need these lines anymore
        // 2: An instance of the Game prefab. Everything is nested under the Game. 👈
        //GameObject gameObject =
        //    Object.Instantiate(Resources.Load<GameObject>("Prefabs/Game"));
        //game = gameObject.GetComponent<Game>();
        // ~UPDATED

        // 3: Spawn the asteroid. It will be moved by it self
        GameObject asteroid = game.GetSpawner().SpawnAsteroid();

        // 4: Keeping track of the initial position is required for the assertion where you verify if the asteroid has moved down. 
        float initialYPos = asteroid.transform.position.y;

        // 5: All Unity unit tests are coroutines, so you need to add a yield return.
        // You’re also adding a time - step of 0.1 seconds to simulate the passage of time that the asteroid should be moving down.
        // If you don’t need to simulate a time-step, you can return a null.
        yield return new WaitForSeconds(0.1f);

        // 6: This is the assertion step where you are asserting that the position of the asteroid is less than the initial position (which means it moved down).
        // Understanding assertions is a key part of unit testing, and NUnit provides different assertion methods.
        // Passing or failing the test is determined by this line. 
        Assert.Less(asteroid.transform.position.y, initialYPos);

        // UPDATED: Because of [TearDown] so we delete or don't need these lines anymore
        // 7:
        // Your mom might not yell at you for leaving a mess after your unit tests are finished, but your other tests might decide to fail because of it.
        // :[ It’s always critical that you clean up(delete or reset) your code after a unit test
        // so that when the next test runs there are no artifacts that can affect that test.
        // Deleting the game object is all you have left to do, 
        // since for each test you’re creating a whole new game instance for the next test. 😹
        //Object.Destroy(game.gameObject);
        // ~UPDATED
    }

    // Second test: The next test will test game over when the ship crashes into an asteroid.
    [UnityTest]
    public IEnumerator GameOverOccursOnAsteroidCollision()
    {
        // UPDATED: Because of [Setup] so we delete or don't need these lines anymore
        GameObject gameObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Game"));
        game = gameObj.GetComponent<Game>();
        //

        GameObject asteroid = game.GetSpawner().SpawnAsteroid();

        // 1: Make asteroid same position as ship to let the collision occurs
        asteroid.transform.position = game.GetShip().transform.position;

        // 2: A time-step is needed to ensure the Physics engine Collision event fires so a 0.1 second wait is returned.
        yield return new WaitForSeconds(0.1f);

        // 3: This is a truth assertion, and it checks that the gameOver flag in the Game script has been set to true.
        // The game code works with this flag being set to true when the ship is destroyed,
        // so you’re testing to make sure this is set to true after the ship has been destroyed.
        Assert.True(game.isGameOver);

        // UPDATED: Because of [TearDown] so we delete or don't need these lines anymore
        //Object.Destroy(game.gameObject);
        // ~UPDATED
    }

    /// <summary>
    /// Test restart new game
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator NewGameRestartsGame()
    {
        // 1: This part of the code sets up this test to have the gameOver bool set to true.
        // When the NewGame method is called, it should set this flag back to false. 
        game.isGameOver = true;
        game.NewGame();

        // 2: Here, you assert that the isGameOver bool is false, which should be the case after a new game is called. 
        Assert.False(game.isGameOver);
        yield return null;
    }

    /// <summary>
    /// Tesst if laser could move up after instantiate or not.
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator LaserMovesUp()
    {
        // 1: This gets a reference to a created laser spawned from the ship. 
        GameObject laser = game.GetShip().SpawnLaser();

        // 2: The initial position is recored so you can verify that it’s moving up. 
        float initialYPos = laser.transform.position.y;

        yield return new WaitForSeconds(0.1f);

        // 3: This assertion is just like the one in the AsteroidsMoveDown unit test,
        // only now you’re asserting that the value is greater (indicating that the laser is moving up). 
        Assert.Greater(laser.transform.position.y, initialYPos);
    }

    /// <summary>
    /// Ensure laser destroy asteroid
    /// </summary>
    /// <returns></returns>
    [UnityTest]
    public IEnumerator LaserDestroysAsteroid()
    {
        // 1: You are creating an asteroid and a laser, and making sure they have the same position so as to trigger a collision. 
        GameObject asteroid = game.GetSpawner().SpawnAsteroid();
        asteroid.transform.position = Vector3.zero;

        GameObject laser = game.GetShip().SpawnLaser();
        laser.transform.position = Vector3.zero;

        yield return new WaitForSeconds(0.1f);

        // 2: A special test with an important distinction.
        // Notice how you are explicitly using UnityEngine.Assertions for this test?
        // That’s because Unity has a special Null class which is different from a “normal” Null class. The NUnit framework assertion Assert.IsNull() will not work for Unity null checks. When checking for nulls in Unity, you must explicitly use the UnityEngine.Assertions.Assert, not the NUnit Assert. 
        UnityEngine.Assertions.Assert.IsNull(asteroid);
    }

    [UnityTest]
    public IEnumerator DestroyedAsteroidRaiseScore()
    {
        // 1: You’re spawning an asteroid and a laser, and making sure they’re in the same position.
        // This ensures a collision occurs, which will trigger a score increase. 
        GameObject asteroid = game.GetSpawner().SpawnAsteroid();
        asteroid.transform.position = Vector3.zero;

        GameObject laser = game.GetShip().SpawnLaser();
        laser.transform.position = Vector3.zero;

        yield return new WaitForSeconds(0.1f);

        // 2: This asserts that the game.score int is now 1 (instead of the 0 that it starts at). 
        Assert.AreEqual(game.score, 1);
    }
}

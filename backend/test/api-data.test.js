import assert from "node:assert/strict";
import { test } from "node:test";
import { authenticate, getFloors, getProjects } from "../lib/data.js";

test("authenticates the assignment demo user", () => {
  const result = authenticate("testuser", "123456");

  assert.equal(result.success, true);
  assert.equal(result.token, "demo_jwt_token_123");
  assert.deepEqual(result.user, { id: 1, name: "Test User" });
});

test("rejects invalid credentials with a clear message", () => {
  const result = authenticate("testuser", "wrong");

  assert.deepEqual(result, {
    success: false,
    message: "Invalid credentials"
  });
});

test("returns four selectable projects", () => {
  const result = getProjects();

  assert.deepEqual(result.projects.map((project) => project.name), [
    "Project A",
    "Project B",
    "Project C",
    "Project D"
  ]);
});

test("returns dynamic floors for each project", () => {
  assert.deepEqual(getFloors(1).floors, ["Floor 1", "Floor 2", "Floor 3", "Floor 4"]);
  assert.deepEqual(getFloors(2).floors, ["Floor A", "Floor B"]);
  assert.deepEqual(getFloors(3).floors, ["Ground", "1", "2", "3", "4", "5"]);
  assert.deepEqual(getFloors(4).floors, ["Basement", "Ground", "Mezzanine"]);
});

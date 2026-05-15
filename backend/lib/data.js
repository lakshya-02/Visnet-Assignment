export const VALID_CREDENTIALS = {
  username: "testuser",
  password: "123456"
};

export const demoUser = {
  id: 1,
  name: "Test User"
};

export const projects = [
  { id: 1, name: "Project A" },
  { id: 2, name: "Project B" },
  { id: 3, name: "Project C" },
  { id: 4, name: "Project D" }
];

export const floorsByProjectId = {
  1: ["Floor 1", "Floor 2", "Floor 3", "Floor 4"],
  2: ["Floor A", "Floor B"],
  3: ["Ground", "1", "2", "3", "4", "5"],
  4: ["Basement", "Ground", "Mezzanine"]
};

export function authenticate(username, password) {
  if (username === VALID_CREDENTIALS.username && password === VALID_CREDENTIALS.password) {
    return {
      success: true,
      token: "demo_jwt_token_123",
      user: demoUser
    };
  }

  return {
    success: false,
    message: "Invalid credentials"
  };
}

export function getProjects() {
  return { projects };
}

export function getFloors(projectId) {
  return {
    floors: floorsByProjectId[String(projectId)] || []
  };
}

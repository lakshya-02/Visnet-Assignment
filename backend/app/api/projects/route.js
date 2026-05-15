import { getProjects } from "../../../lib/data";

export async function GET() {
  return Response.json(getProjects());
}

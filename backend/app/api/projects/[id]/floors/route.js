import { getFloors } from "../../../../../lib/data";

export async function GET(_req, { params }) {
  return Response.json(getFloors(params.id));
}

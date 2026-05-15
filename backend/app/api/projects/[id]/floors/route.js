import { getFloors } from "../../../../../lib/data";

export async function GET(_req, { params }) {
  const { id } = await params;
  return Response.json(getFloors(id));
}

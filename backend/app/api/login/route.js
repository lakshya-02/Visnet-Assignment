import { authenticate } from "../../../lib/data";

export async function POST(req) {
  const body = await req.json();
  const result = authenticate(body.username, body.password);

  if (!result.success) {
    return Response.json(result, { status: 401 });
  }

  return Response.json(result);
}

from fastapi import FastAPI, Response
from captcha.image import ImageCaptcha
from io import BytesIO

image_captcha = ImageCaptcha()
app = FastAPI()

@app.get("/GenerateCaptcha/{code}")
async def generate_captcha(code: str):
    print(code)
    data = image_captcha.generate(code)
    return Response(data.getvalue(), media_type="image/png")
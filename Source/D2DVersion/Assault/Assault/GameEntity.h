#pragma once
ref class GameEntity
{
internal:
  GameEntity(void);
public:
  virtual void Initialize() = 0;
  void Render(
        _In_ ID3D11DeviceContext *context,
        _In_ ID3D11Buffer *primitiveConstantBuffer
        );

protected private:  
    Microsoft::WRL::ComPtr<ID2D1Bitmap>  m_sprite;    
};


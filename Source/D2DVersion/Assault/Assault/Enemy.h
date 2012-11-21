#pragma once
#include "GameEntity.h"

ref class Enemy : public GameEntity
{
internal:
  Enemy(void);

public:
  virtual void Initialize() override;
  
};


<?php

namespace Database\Factories;

use App\Models\User;
use Illuminate\Database\Eloquent\Factories\Factory;
use Illuminate\Support\Str;

class UserFactory extends Factory
{
    /**
     * The name of the factory's corresponding model.
     *
     * @var string
     */
    protected $model = User::class;

    /**
     * Define the model's default state.
     *
     * @return array
     */
    public function definition()
    {
        return [
            'name' => $this->faker->name,
            'lat' => $this->faker->latitude(50,51),
            'lon' => $this->faker->longitude(50,51),
            'sex' => $this->faker->randomElement(['мужской', 'женский', 'другое']),
            'date_birth' => $this->faker->date,
        ];
    }
}

<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateStats extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('stats', function (Blueprint $table) {
			$table->bigInteger("stat_id")->autoIncrement()->nullable(false)->index();
			$table->bigInteger("user_id")->nullable(false)->index();
			$table->tinyInteger("mode")->default(0)->nullable(false)->index();
			$table->bigInteger("ranked_score")->default(0)->nullable(false);
			$table->bigInteger("total_score")->default(0)->nullable(false);
			$table->float("performance")->default(0)->nullable(false);
			$table->float("level")->default(0)->nullable(false);
			$table->float("accuracy")->default(0)->nullable(false);
			$table->bigInteger("playcount")->default(0)->nullable(false);
			$table->bigInteger("count_ssh")->default(0)->nullable(false);
			$table->bigInteger("count_ss")->default(0)->nullable(false);
			$table->bigInteger("count_sh")->default(0)->nullable(false);
			$table->bigInteger("count_s")->default(0)->nullable(false);
			$table->bigInteger("count_a")->default(0)->nullable(false);
			$table->bigInteger("count_b")->default(0)->nullable(false);
			$table->bigInteger("count_c")->default(0)->nullable(false);
			$table->bigInteger("count_d")->default(0)->nullable(false);
			$table->bigInteger("hit300")->default(0)->nullable(false);
			$table->bigInteger("hit100")->default(0)->nullable(false);
			$table->bigInteger("hit50")->default(0)->nullable(false);
			$table->bigInteger("hitGeki")->default(0)->nullable(false);
			$table->bigInteger("hitKatu")->default(0)->nullable(false);
			$table->bigInteger("hitMiss")->default(0)->nullable(false);
            $table->timestamps();
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('stats');
    }
}

<?php

use Illuminate\Database\Migrations\Migration;
use Illuminate\Database\Schema\Blueprint;
use Illuminate\Support\Facades\Schema;

class CreateBeatmaps extends Migration
{
    /**
     * Run the migrations.
     *
     * @return void
     */
    public function up()
    {
        Schema::create('beatmaps', function (Blueprint $table) {
            $table->bigInteger("beatmap_id")->autoIncrement()->nullable(false);
            $table->string("map_md5", 32)->index()->nullable(false)->unique();
            $table->bigInteger("bancho_id")->index();
            $table->bigInteger("beatmapset_id")->index()->default(0)->nullable(false);
            $table->string("difficulty_name")->index()->default("")->nullable(false);
            $table->timestamp("last_update")->default(DB::raw('CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP'))->nullable(false);
            $table->timestamp("creation_time")->default(DB::raw('CURRENT_TIMESTAMP'))->nullable(false);
            $table->float("difficulty_hp")->default(0.0)->nullable(false);
            $table->float("difficulty_ar")->default(0.0)->nullable(false);
            $table->float("difficulty_cs")->default(0.0)->nullable(false);
            $table->float("difficulty_od")->default(0.0)->nullable(false);
            $table->float("difficulty_sr")->default(0.0)->nullable(false);
            $table->float("difficulty_bpm")->default(0.0)->nullable(false);
            $table->float("difficulty_sm")->default(0.0)->nullable(false);
            $table->tinyInteger("mode")->default(0)->nullable(false);
            $table->bigInteger("playcount")->default(0)->nullable(false);
           	$table->bigInteger("passcount")->default(0)->nullable(false);
           	$table->integer("length")->default(0)->nullable(false);
           	$table->integer("count_normal")->default(0)->nullable(false);
           	$table->integer("count_sliders")->default(0)->nullable(false);
           	$table->integer("count_spinners")->default(0)->nullable(false);
           	$table->boolean("storyboard")->default(false)->nullable(false);
           	$table->boolean("video")->default(false)->nullable(false);
        });
    }

    /**
     * Reverse the migrations.
     *
     * @return void
     */
    public function down()
    {
        Schema::dropIfExists('beatmaps');
    }
}
